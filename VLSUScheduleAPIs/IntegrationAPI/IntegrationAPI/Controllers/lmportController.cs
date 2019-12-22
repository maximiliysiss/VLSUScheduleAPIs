using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AuthAPI.Services;
using ClosedXML.Excel;
using Commonlibrary.Models;
using CommonLibrary.Models;
using IntegrationAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetServiceConnection.NetContext;

namespace IntegrationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly VlsuContext vlsuContext;
        private readonly RusDayOfWeekService rusDayOfWeekService;

        public ImportController(ILogger<ImportController> logger, VlsuContext vlsuContext, RusDayOfWeekService rusDayOfWeekService)
        {
            this.logger = logger;
            this.vlsuContext = vlsuContext;
            this.rusDayOfWeekService = rusDayOfWeekService;
        }

        private T CreateOrGet<T>(NetSet<T> os, Func<T, bool> filter, Func<T> construct)
        {
            var elem = os.FirstOrDefault(x => filter(x));
            if (elem == null)
            {
                var newElem = construct();
                os.Add(newElem);
                vlsuContext.Commit();
                return newElem;
            }
            return elem;
        }

        private T CreateOrUpdate<T>(NetSet<T> os, Func<T, bool> filter, Func<T> construct, Action<T, int> setId, Func<T, int> getId) where T : class
        {
            var elem = os.FirstOrDefault(x => filter(x));
            var newElem = construct();
            if (elem == null)
                os.Add(newElem);
            else
            {
                setId(newElem, getId(elem));
                os.Update(newElem);
            }
            vlsuContext.Commit();
            return elem ?? newElem;
        }

        [HttpPost("excel")]
        public ActionResult UploadScheduleExcel(IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest();
            logger.LogInformation("Start loading excel document");

            try
            {
                var name = Path.GetFileNameWithoutExtension(file.FileName).ToLower();
                var institute = CreateOrGet(vlsuContext.Institutes, x => x.Name == name, () => new Institute { Name = name });

                using (var workBook = new XLWorkbook(file.OpenReadStream()))
                {
                    foreach (var sheet in workBook.Worksheets)
                    {
                        var groupsNameList = sheet.Rows(2, 2).Cells().Skip(2).Select(x => x.Value.ToString().Trim()).ToList();
                        Dictionary<Group, int> groupMask = new Dictionary<Group, int>();
                        Group prevGroup = null;
                        for (int i = 0; i < groupsNameList.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(groupsNameList[i]))
                            {
                                prevGroup = CreateOrGet(vlsuContext.Groups, x => x.Name == groupsNameList[i], () => new Group { Name = groupsNameList[i], InstituteId = institute.ID });
                                groupMask[prevGroup] = 1;
                            }
                            else
                                groupMask[prevGroup]++;
                        }

                        string prevDate = string.Empty;
                        string prevTime = string.Empty;

                        foreach (var row in sheet.Rows().Skip(3))
                        {
                            var dayString = row.Cell(1).Value.ToString();
                            var day = rusDayOfWeekService[prevDate = (dayString.Length == 0 ? prevDate : dayString)];
                            var timeString = row.Cell(2).Value.ToString();
                            var time = DateTime.Parse(prevTime = (timeString.Length == 0 ? prevTime : timeString.Split(' ')[0]));
                            bool isOdd = timeString.Length > 0;
                            var values = row.Cells().Skip(2).Select(x => new
                            {
                                Data = x.Value.ToString(),
                                Range = x.IsMerged() ? x.MergedRange().FirstCell().Address.ColumnNumber : x.Address.ColumnNumber
                            }).ToList();

                            foreach (var group in groupMask.Select((x, i) => new { x, i }))
                            {
                                var skipColumns = groupMask.Take(group.i).Sum(x => x.Value);
                                var cellsDatas = values.Skip(skipColumns).Take(group.x.Value).ToList();
                                int subGroup = 1;

                                var realCells = cellsDatas.Where(x => !string.IsNullOrEmpty(x.Data)).ToList();

                                if (realCells.Count == 0 && cellsDatas.Count > 0 && cellsDatas[0].Range != skipColumns + 2)
                                {
                                    var data = row.Cell(cellsDatas[0].Range).Value.ToString();
                                    if (!string.IsNullOrEmpty(data))
                                        realCells.Add(new { Data = data, Range = 0 });
                                }


                                foreach (var cellData in realCells)
                                {
                                    var splitData = cellData.Data.Split(' ');
                                    var auditoryData = splitData.Select((x, i) => new { x, i }).FirstOrDefault(x => x.x.Any(char.IsDigit));
                                    string auditoryString = auditoryData?.x;
                                    string teacherString = string.Empty;
                                    if (string.IsNullOrEmpty(auditoryString))
                                    {
                                        auditoryString = "Не определена";
                                        teacherString = "Не установлен";
                                    }
                                    else
                                    {
                                        teacherString = string.Join(" ", splitData.Skip(auditoryData.i + 1));
                                    }

                                    var lessonString = string.Join(" ", splitData.Take(auditoryData?.i ?? splitData.Length));
                                    var lesson = CreateOrGet(vlsuContext.Lessons, (x) => x.Name == lessonString, () => new Lesson { Name = lessonString });

                                    var auditory = CreateOrGet(vlsuContext.Auditories, (x) => x.Name == auditoryString, () => new Auditory { Name = auditoryString });
                                    var teacher = CreateOrGet(vlsuContext.Teachers, (x) => x.ShortName?.ToLower() == teacherString.ToLower(),
                                        () => new Teacher
                                        {
                                            FIO = teacherString,
                                            Login = $"teacher_{Guid.NewGuid()}",
                                            UserType = UserType.Teacher,
                                            ShortName = teacherString,
                                            Password = CryptService.CreateMD5($"teacher_{teacherString}")
                                        });


                                    var schedule = CreateOrUpdate(vlsuContext.Schedules,
                                        (x) => x.GroupId == group.x.Key.ID && x.Odd == isOdd && x.DayOfWeek == day && x.SubGroup == subGroup && x.Time == time,
                                        () => new Schedule
                                        {
                                            AuditoryId = auditory.ID,
                                            DayOfWeek = day,
                                            GroupId = group.x.Key.ID,
                                            Odd = isOdd,
                                            SubGroup = subGroup,
                                            TeacherId = teacher.ID,
                                            Time = time,
                                            LessonId = lesson.ID
                                        }, (x, id) => x.ID = id, (x) => x.ID);

                                    subGroup++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            logger.LogInformation("End loading excel document");

            return Ok();
        }

        [HttpPost("json")]
        public ActionResult UploadScheduleJson()
        {
            return Ok();
        }

        [HttpPost("xml")]
        public ActionResult UploadScheduleXML()
        {
            return Ok();
        }
    }
}