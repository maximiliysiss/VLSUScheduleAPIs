using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Commonlibrary.Models;
using IntegrationAPI.Services;
using AuthAPI.Services;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly VlsuContext _context;

        public TeachersController(VlsuContext context)
        {
            _context = context;
        }

        // GET: api/Teachers
        [HttpGet]
        public ActionResult<IEnumerable<Teacher>> GetTeachers()
        {
            return _context.Teachers.ToList();
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        public ActionResult<Teacher> GetTeacher(int id)
        {
            var teacher = _context.Teachers.Get(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        // PUT: api/Teachers/5
        [HttpPut("{id}")]
        public IActionResult PutTeacher(int id, Teacher teacher)
        {
            if (id != teacher.ID)
            {
                return BadRequest();
            }

            teacher.UserType = UserType.Teacher;
            var currentTeacher = _context.Teachers.Get(id);
            if (currentTeacher != null && currentTeacher.Password != teacher.Password)
                teacher.Password = CryptService.CreateMD5(teacher.Password);

            _context.Teachers.Update(teacher);
            _context.Commit();

            return NoContent();
        }

        // POST: api/Teachers
        [HttpPost]
        public ActionResult<Teacher> PostTeacher(Teacher teacher)
        {
            teacher.UserType = UserType.Teacher;
            teacher.Password = CryptService.CreateMD5(teacher.Password);
            _context.Teachers.Add(teacher);
            _context.Commit();

            return CreatedAtAction("GetTeacher", new { id = teacher.ID }, teacher);
        }

        // DELETE: api/Teachers/5
        [HttpDelete("{id}")]
        public ActionResult<Teacher> DeleteTeacher(int id)
        {
            var teacher = _context.Teachers.Get(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            _context.Commit();

            return teacher;
        }
    }
}
