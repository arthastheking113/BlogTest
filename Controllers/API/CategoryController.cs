using BlogTest.Data;
using BlogTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Controllers.API
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getcategory")]
        public async Task<ActionResult<IEnumerable<BlogCategory>>> GetContact()
        {
            var CategoryList = await _context.BlogCategory.ToListAsync();
            return CategoryList;
        }
    }
}
