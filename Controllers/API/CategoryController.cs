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
        public async Task<ActionResult<IEnumerable<Category>>> GetContact()
       {
            var CategoryList = await _context.BlogCategory.ToListAsync();
            List<Category> categories = new List<Category>();
            foreach (var item in CategoryList)
            {
                Category category = new Category
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                    UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt")
                };
                categories.Add(category);
            }
            return Ok(categories);
        }


    }
}
