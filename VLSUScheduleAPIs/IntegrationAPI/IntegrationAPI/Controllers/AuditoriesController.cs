using System.Collections.Generic;
using System.Linq;
using Commonlibrary.Models;
using IntegrationAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriesController : ControllerBase
    {
        /// <summary>
        /// NetContext
        /// </summary>
        private readonly VlsuContext _context;

        public AuditoriesController(VlsuContext _context)
        {
            this._context = _context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Auditory>> GetAuditories()
        {
            return _context.Auditories.ToList();
        }

        // GET: api/Auditories/5
        [HttpGet("{id}")]
        public ActionResult<Auditory> GetAuditory(int id)
        {
            var auditory = _context.Auditories.Get(id);

            if (auditory == null)
            {
                return NotFound();
            }

            return auditory;
        }

        // PUT: api/Auditories/5
        [HttpPut("{id}")]
        public IActionResult PutAuditory(int id, Auditory auditory)
        {
            if (id != auditory.ID)
            {
                return BadRequest();
            }

            var currentAuditory = _context.Auditories.Get(id);
            if (currentAuditory == null)
                return NotFound();

            _context.Auditories.Update(auditory);
            _context.Commit();

            return NoContent();
        }

        // POST: api/Auditories
        [HttpPost]
        public ActionResult<Auditory> PostAuditory(Auditory auditory)
        {
            _context.Auditories.Add(auditory);
            _context.Commit();

            return CreatedAtAction("GetAuditory", new { id = auditory.ID }, auditory);
        }

        // DELETE: api/Auditories/5
        [HttpDelete("{id}")]
        public ActionResult<Auditory> DeleteAuditory(int id)
        {
            var auditory = _context.Auditories.Get(id);
            if (auditory == null)
            {
                return NotFound();
            }

            _context.Auditories.Remove(auditory);
            _context.Commit();

            return auditory;
        }
    }
}