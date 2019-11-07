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
    public class GroupsController : ControllerBase
    {
        VlsuContext _context;

        public GroupsController(VlsuContext context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            return _context.Groups.ToList();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var @group = _context.Groups.Get(id);

            if (@group == null)
            {
                return NotFound();
            }

            return @group;
        }

        // PUT: api/Groups/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group @group)
        {
            if (id != @group.ID)
            {
                return BadRequest();
            }

            _context.Groups.Update(@group);

            _context.Commit();

            return NoContent();
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group @group)
        {
            _context.Groups.Add(@group);
            _context.Commit();

            return CreatedAtAction("GetGroup", new { id = @group.ID }, @group);
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Group>> DeleteGroup(int id)
        {
            var @group = _context.Groups.Get(id);
            if (@group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(@group);
            _context.Commit();

            return @group;
        }
    }
}