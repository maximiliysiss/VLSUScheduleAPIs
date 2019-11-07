﻿using System;
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
    public class IllCardsController : ControllerBase
    {
        VlsuContext _context;

        public IllCardsController(VlsuContext context)
        {
            _context = context;
        }

        // GET: api/IllCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IllCard>>> GetIllCards()
        {
            return _context.IllCards.ToList();
        }

        // GET: api/IllCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IllCard>> GetIllCard(int id)
        {
            var illCard = _context.IllCards.Get(id);

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

            _context.IllCards.Update(illCard);
            _context.Commit();

            return NoContent();
        }

        // POST: api/IllCards
        [HttpPost]
        public async Task<ActionResult<IllCard>> PostIllCard(IllCard illCard)
        {
            _context.IllCards.Add(illCard);
            _context.Commit();

            return CreatedAtAction("GetIllCard", new { id = illCard.ID }, illCard);
        }

        // DELETE: api/IllCards/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IllCard>> DeleteIllCard(int id)
        {
            var illCard = _context.IllCards.Get(id);
            if (illCard == null)
            {
                return NotFound();
            }

            _context.IllCards.Remove(illCard);
            _context.Commit();

            return illCard;
        }
    }
}