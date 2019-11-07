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
    public class LessonsController : ControllerBase
    {
        VlsuContext _context;

        public LessonsController(VlsuContext context)
        {
            _context = context;
        }

        // GET: api/Lessons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons()
        {
            return _context.Lessons.ToList();
        }

        // GET: api/Lessons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = _context.Lessons.Get(id);

            if (lesson == null)
            {
                return NotFound();
            }

            return lesson;
        }

        // PUT: api/Lessons/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLesson(int id, Lesson lesson)
        {
            if (id != lesson.ID)
            {
                return BadRequest();
            }

            _context.Lessons.Update(lesson);
            _context.Commit();

            return NoContent();
        }

        // POST: api/Lessons
        [HttpPost]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            _context.Commit();

            return CreatedAtAction("GetLesson", new { id = lesson.ID }, lesson);
        }

        // DELETE: api/Lessons/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Lesson>> DeleteLesson(int id)
        {
            var lesson = _context.Lessons.Get(id);
            if (lesson == null)
            {
                return NotFound();
            }

            _context.Lessons.Remove(lesson);
            _context.Commit();

            return lesson;
        }
    }
}