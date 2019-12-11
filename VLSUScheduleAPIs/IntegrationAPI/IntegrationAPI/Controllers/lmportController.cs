using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Commonlibrary.Models;
using IntegrationAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        [HttpPost("excel")]
        public ActionResult UploadScheduleExcel(IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest();
            logger.LogInformation("Start loading excel document");

            try
            {
                var name = Path.GetFileNameWithoutExtension(file.FileName).ToLower();
                var institute = vlsuContext.Institutes.FirstOrDefault(x => x.Name.ToLower() == name);
                if (institute == null)
                {
                    institute = new CommonLibrary.Models.Institute { Name = name };
                    vlsuContext.Institutes.Add(institute);
                    vlsuContext.Commit();
                }

                using (var workBook = new XLWorkbook(file.OpenReadStream()))
                {
                    foreach (var row in workBook.Worksheets.ToList()[0].Rows().Skip(1))
                    {
                        var auditoryName = row.Cell(1).Value.ToString();
                        var lessonName = row.Cell(2).Value.ToString();
                        var teacherName = row.Cell(3).Value.ToString();
                        var groupName = row.Cell(4).Value.ToString();
                        var subGroup = int.Parse(row.Cell(5).Value.ToString());
                        var time = DateTime.Parse(row.Cell(6).Value.ToString());
                        var day = row.Cell(7).Value.ToString();
                        var odd = row.Cell(8).Value.ToString();
                        var auditory = vlsuContext.Auditories.FirstOrDefault(x => x.Name == auditoryName);
                        if (auditory == null)
                        {
                            auditory = new Auditory { Name = auditoryName };
                            vlsuContext.Auditories.Add(auditory);
                            vlsuContext.Commit();
                        }

                        var lesson = vlsuContext.Lessons.FirstOrDefault(x => x.Name == lessonName);
                        if (lesson == null)
                        {
                            lesson = new Lesson { Name = lessonName };
                            vlsuContext.Lessons.Add(lesson);
                            vlsuContext.Commit();
                        }

                        var teacher = vlsuContext.Teachers.FirstOrDefault(x => x.ShortName == teacherName);
                        if (teacher == null)
                        {
                            teacher = new Teacher
                            {
                                Birthday = DateTime.Today,
                                FIO = teacherName,
                                Login = teacherName,
                                Password = Guid.NewGuid().ToString(),
                                ShortName = teacherName,
                                UserType = UserType.Teacher
                            };
                            vlsuContext.Teachers.Add(teacher);
                            vlsuContext.Commit();
                        }

                        var group = vlsuContext.Groups.FirstOrDefault(x => x.Name == groupName && x.InstituteId == institute.ID);
                        if (group == null)
                        {
                            group = new Group { InstituteId = institute.ID, Name = groupName };
                            vlsuContext.Groups.Add(group);
                            vlsuContext.Commit();
                        }

                        var newSchedule = new Schedule
                        {
                            AuditoryId = auditory.ID,
                            LessonId = lesson.ID,
                            TeacherId = teacher.ID,
                            GroupId = group.ID,
                            SubGroup = subGroup,
                            Time = time,
                            DayOfWeek = rusDayOfWeekService[day],
                            Odd = YesNoService.YesNo[odd]
                        };

                        var existSchedule = vlsuContext.Schedules.FirstOrDefault(x => x.GroupId == newSchedule.GroupId && x.DayOfWeek == newSchedule.DayOfWeek
                                                                                && x.Odd == newSchedule.Odd && x.SubGroup == newSchedule.SubGroup
                                                                                && x.Time == newSchedule.Time);
                        if (existSchedule == null)
                            vlsuContext.Schedules.Add(newSchedule);
                        else
                        {
                            newSchedule.ID = existSchedule.ID;
                            vlsuContext.Schedules.Update(newSchedule);
                        }

                        vlsuContext.Commit();
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