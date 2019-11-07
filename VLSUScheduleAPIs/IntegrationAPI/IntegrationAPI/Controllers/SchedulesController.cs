using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commonlibrary.Models;
using IntegrationAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        VlsuContext _context;

        public SchedulesController(VlsuContext context)
        {
            _context = context;
        }

        // GET: api/Schedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            return _context.Schedules.ToList();
        }

        // GET: api/Schedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = _context.Schedules.Get(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return schedule;
        }

        // PUT: api/Schedules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchedule(int id, Schedule schedule)
        {
            if (id != schedule.ID)
            {
                return BadRequest();
            }

            _context.Schedules.Update(schedule);
            _context.Commit();

            return NoContent();
        }

        // POST: api/Schedules
        [HttpPost]
        public async Task<ActionResult<Schedule>> PostSchedule(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            _context.Commit();

            return CreatedAtAction("GetSchedule", new { id = schedule.ID }, schedule);
        }

        // DELETE: api/Schedules/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Schedule>> DeleteSchedule(int id)
        {
            var schedule = _context.Schedules.Get(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(schedule);
            _context.Commit();

            return schedule;
        }
    }
}