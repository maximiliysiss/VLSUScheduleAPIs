using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;
using VLSUScheduleAPIs.Services;
using Microsoft.AspNetCore.Authorization;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeAttribute(Roles = "Teacher, Service")]
    public class IllCardsController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleChanger scheduleChanger;

        public IllCardsController(DatabaseContext context, ScheduleChanger scheduleChanger)
        {
            _context = context;
            this.scheduleChanger = scheduleChanger;
        }



        // GET: api/IllCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IllCard>>> GetIllCards()
        {
            return await _context.IllCards.ToListAsync();
        }

        // GET: api/IllCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IllCard>> GetIllCard(int id)
        {
            var illCard = await _context.IllCards.FindAsync(id);

            if (illCard == null)
                return NotFound();

            return illCard;
        }

        // POST: api/IllCards
        [HttpPost]
        public async Task<ActionResult<IllCard>> PostIllCard(IllCard illCard)
        {
            _context.IllCards.Add(illCard);
            await _context.SaveChangesAsync();
            scheduleChanger.Reload("ill", illCard);
            return CreatedAtAction("GetIllCard", new { id = illCard.ID }, illCard);
        }

        // DELETE: api/IllCards/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IllCard>> DeleteIllCard(int id)
        {
            var illCard = await _context.IllCards.FindAsync(id);
            if (illCard == null)
                return NotFound();

            illCard.IsDelete = true;
            _context.IllCards.Update(illCard);
            await _context.SaveChangesAsync();
            return illCard;
        }
    }
}
