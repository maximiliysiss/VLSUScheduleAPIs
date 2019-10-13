using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;
using VLSUScheduleAPIs.Services;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IllCardsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public IllCardsController(DatabaseContext context)
        {
            _context = context;
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
            {
                return NotFound();
            }

            return illCard;
        }

        // PUT: api/IllCards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIllCard(int id, IllCard illCard)
        {
            if (id != illCard.ID)
            {
                return BadRequest();
            }

            _context.Entry(illCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IllCardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/IllCards
        [HttpPost]
        public async Task<ActionResult<IllCard>> PostIllCard(IllCard illCard)
        {
            _context.IllCards.Add(illCard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIllCard", new { id = illCard.ID }, illCard);
        }

        // DELETE: api/IllCards/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IllCard>> DeleteIllCard(int id)
        {
            var illCard = await _context.IllCards.FindAsync(id);
            if (illCard == null)
            {
                return NotFound();
            }

            _context.IllCards.Remove(illCard);
            await _context.SaveChangesAsync();

            return illCard;
        }

        private bool IllCardExists(int id)
        {
            return _context.IllCards.Any(e => e.ID == id);
        }
    }
}
