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
using Microsoft.Extensions.Logging;
using Commonlibrary.Services.Settings;
using Commonlibrary.Service;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Collections;

namespace VLSUScheduleAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeAttribute(Roles = "Service")]
    public class AuditoriesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IServiceConnection rabbitOnCreate;
        private readonly IServiceConnection rabbitOnEdit;
        private readonly IServiceConnection rabbitOnSelectAll;

        public AuditoriesController(DatabaseContext context, IServiceConnectionFactory factory, RabbitSettings rabbitSettings)
        {
            _context = context;
            rabbitOnCreate = factory.Create(rabbitSettings.Topic, "rabbit:auditory:create");
            rabbitOnEdit = factory.Create(rabbitSettings.Topic, "rabbit:auditory:edit");
            rabbitOnSelectAll = factory.Create(rabbitSettings.Topic, "rabbit:auditory:all");
            rabbitOnCreate.RegisterConsumer<Auditory>(async x => await PostAuditory(x));
            rabbitOnEdit.RegisterConsumer<Auditory>(async x => await PutAuditory(x.ID, x));
            rabbitOnSelectAll.BackwayConsumer("loadall", () => GetAuditories().Result.Value, factory);
            rabbitOnSelectAll.BackwayConsumer<Auditory, int>("load", (x) => GetAuditory(x).Result.Value, factory);
        }

        // GET: api/Auditories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auditory>>> GetAuditories()
        {
            return await _context.Auditories.ToListAsync();
        }

        // GET: api/Auditories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Auditory>> GetAuditory(int id)
        {
            var auditory = await _context.Auditories.FindAsync(id);

            if (auditory == null)
            {
                return NotFound();
            }

            return auditory;
        }

        // PUT: api/Auditories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuditory(int id, Auditory auditory)
        {
            if (id != auditory.ID)
            {
                return BadRequest();
            }

            _context.Entry(auditory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditoryExists(id))
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

        // POST: api/Auditories
        [HttpPost]
        public async Task<ActionResult<Auditory>> PostAuditory(Auditory auditory)
        {
            _context.Auditories.Add(auditory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuditory", new { id = auditory.ID }, auditory);
        }

        // DELETE: api/Auditories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Auditory>> DeleteAuditory(int id)
        {
            var auditory = await _context.Auditories.FindAsync(id);
            if (auditory == null)
            {
                return NotFound();
            }

            _context.Auditories.Remove(auditory);
            await _context.SaveChangesAsync();

            return auditory;
        }

        private bool AuditoryExists(int id)
        {
            return _context.Auditories.Any(e => e.ID == id);
        }
    }
}
