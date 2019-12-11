using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;
using VLSUScheduleAPIs.Services;
using Microsoft.AspNetCore.Authorization;
using Commonlibrary.Controllers;
using System.Linq;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher, Service")]
    public class IllCardsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleChanger scheduleChanger;

        public IllCardsController(DatabaseContext context, ScheduleChanger scheduleChanger)
        {
            _context = context;
            this.scheduleChanger = scheduleChanger;
        }

        private bool IsTeacher
        {
            get
            {
                var userType = this.UserType();
                return userType != null && userType == UserType.Teacher;
            }
        }


        // GET: api/IllCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IllCard>>> GetIllCards()
        {
            if (IsTeacher)
                return await _context.IllCards.Where(x => x.TeacherId == this.UserId()).ToListAsync();
            return await _context.IllCards.ToListAsync();
        }

        // GET: api/IllCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IllCard>> GetIllCard(int id)
        {
            var illCard = await _context.IllCards.FindAsync(id);

            if (illCard == null || (IsTeacher && illCard.TeacherId != this.UserId()))
                return NotFound();

            return illCard;
        }

        // POST: api/IllCards
        [HttpPost]
        public async Task<ActionResult<IllCard>> PostIllCard(IllCard illCard)
        {
            if (IsTeacher)
                illCard.TeacherId = this.UserId().Value;
            var schedule = _context.Schedules.FirstOrDefault(x => x.ID == illCard.ScheduleId);
            if (schedule == null || schedule.TeacherId != illCard.TeacherId)
                return NotFound();

            _context.IllCards.Add(illCard);
            await _context.SaveChangesAsync();
            scheduleChanger.Reload("vlsu.ill.add", illCard);
            return CreatedAtAction("GetIllCard", new { id = illCard.ID }, illCard);
        }

        // DELETE: api/IllCards/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IllCard>> DeleteIllCard(int id)
        {
            var illCard = await _context.IllCards.FindAsync(id);
            if (illCard == null || (IsTeacher && illCard.TeacherId != this.UserId()))
                return NotFound();

            illCard.IsDelete = true;
            _context.IllCards.Update(illCard);
            await _context.SaveChangesAsync();
            scheduleChanger.Reload("vlsu.ill.delete", illCard);
            return illCard;
        }
    }
}
