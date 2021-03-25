using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogTest.Data;
using BlogTest.Models;

namespace BlogTest.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostCategory>>> GetPostCategory()
        {
            return await _context.PostCategory.ToListAsync();
        }

        // GET: api/PostCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostCategory>> GetPostCategory(int id)
        {
            var postCategory = await _context.PostCategory.FindAsync(id);

            if (postCategory == null)
            {
                return NotFound();
            }

            return postCategory;
        }

        [HttpGet("/GetTopXPost/{num}")]
        public async Task<ActionResult<IEnumerable<PostCategory>>> GetTopXPost(int num)
        {
            return await _context.PostCategory.OrderByDescending(p => p.CreateDate).Take(num).ToListAsync();
        }


        // PUT: api/PostCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostCategory(int id, PostCategory postCategory)
        {
            if (id != postCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(postCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostCategoryExists(id))
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

        // POST: api/PostCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PostCategory>> PostPostCategory(PostCategory postCategory)
        {
            _context.PostCategory.Add(postCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostCategory", new { id = postCategory.Id }, postCategory);
        }

        // DELETE: api/PostCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostCategory(int id)
        {
            var postCategory = await _context.PostCategory.FindAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }

            _context.PostCategory.Remove(postCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategory.Any(e => e.Id == id);
        }
    }
}
