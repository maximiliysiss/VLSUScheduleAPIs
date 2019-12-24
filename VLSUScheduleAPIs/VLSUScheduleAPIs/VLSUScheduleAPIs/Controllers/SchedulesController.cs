﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commonlibrary.Models;
using VLSUScheduleAPIs.Services;
using Microsoft.AspNetCore.Authorization;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Service")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SchedulesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ScheduleChanger scheduleChanger;

        public SchedulesController(DatabaseContext context, ScheduleChanger scheduleChanger)
        {
            _context = context;
            this.scheduleChanger = scheduleChanger;
        }

        // GET: api/Schedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            return await _context.Schedules.ToListAsync();
        }

        // GET: api/Schedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

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

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            scheduleChanger.Reload("vlsu.schedule.change", schedule);
            return NoContent();
        }

        // POST: api/Schedules
        [HttpPost]
        public async Task<ActionResult<Schedule>> PostSchedule(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            scheduleChanger.Reload("vlsu.schedule.add", schedule);
            return CreatedAtAction("GetSchedule", new { id = schedule.ID }, schedule);
        }

        // DELETE: api/Schedules/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Schedule>> DeleteSchedule(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            scheduleChanger.Reload("vlsu.schedule.delete", schedule, ReloadType.Delete);
            return schedule;
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ID == id);
        }
    }
}
