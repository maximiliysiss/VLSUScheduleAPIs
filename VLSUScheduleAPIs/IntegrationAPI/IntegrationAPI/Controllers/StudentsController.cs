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
    public class StudentsController : ControllerBase
    {
        /// <summary>
        /// NetContext
        /// </summary>
        private readonly VlsuContext _context;

        public StudentsController(VlsuContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            return _context.Students.ToList();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public ActionResult<Student> GetStudent(int id)
        {
            var student = _context.Students.Get(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, Student student)
        {
            if (id != student.ID)
                return BadRequest();

            student.UserType = UserType.Student;
            var currentStudent = _context.Students.Get(id);
            if (currentStudent == null)
                return NotFound();
            if (currentStudent.Password != student.Password)
                student.Password = CryptService.CreateMD5(student.Password);

            _context.Students.Update(student);
            _context.Commit();
            return NoContent();
        }

        // POST: api/Students
        [HttpPost]
        public ActionResult<Student> PostStudent(Student student)
        {
            student.UserType = UserType.Student;
            student.Password = CryptService.CreateMD5(student.Password);
            _context.Students.Add(student);
            _context.Commit();

            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public ActionResult<Student> DeleteStudent(int id)
        {
            var student = _context.Students.Get(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            _context.Commit();

            return student;
        }
    }
}
