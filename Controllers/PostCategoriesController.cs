using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogTest.Data;
using BlogTest.Models;
using Microsoft.AspNetCore.Authorization;
using BlogTest.Service;
using Microsoft.AspNetCore.Http;
using System.IO;
using PagedList;
namespace BlogTest.Controllers
{
    public class PostCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;

        public PostCategoriesController(ApplicationDbContext context, ISlugService slugService, IImageService imageService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
        }

        // GET: PostCategories
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PostCategory.Include(p => p.BlogCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PostCategories/Details/5
        public async Task<IActionResult> CategoryIndexAsync(int? id, int? page, string search)
        {
            page ??= 1;
            var pageSize = 9;
            var totalPage = 0;
            var totalRecord = 0;

            if (search != null)
            {
                search = search.ToLower();
                var postSearch = await _context.PostCategory.Include(h => h.PostComments).Where(cp => cp.BlogCategoryId == id).Where(y => y.IsproductionReady).Where(x => x.Title.ToLower().Contains(search) ||
                                                                x.Content.ToLower().Contains(search) ||
                                                                x.Abstract.ToLower().Contains(search) ||
                                                                x.BlogCategory.Name.ToLower().Contains(search) ||
                                                                x.BlogCategory.Description.ToLower().Contains(search) ||
                                                                x.PostComments.Any(c => c.Content.ToLower().Contains(search) ||
                                                                                    c.BlogUser.FirstName.ToLower().Contains(search) ||
                                                                                    c.BlogUser.LastName.ToLower().Contains(search) ||
                                                                                    c.BlogUser.Email.ToLower().Contains(search))).ToListAsync();
                postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                totalRecord = postSearch.Count;
                totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                var skipCount = ((int)page - 1) * pageSize;
                var posts = postSearch.Skip(skipCount).Take(pageSize);
                ViewData["search"] = search;
                ViewData["totalPage"] = totalPage;
                ViewData["page"] = page;
                return View("Index", posts);
            }
            else
            {
                var postSearch = await _context.PostCategory.Include(h => h.PostComments).Include(p => p.BlogCategory).Where(cp => cp.BlogCategoryId == id).Where(y => y.IsproductionReady).ToListAsync();
                postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                totalRecord = postSearch.Count;
                totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                var skipCount = ((int)page - 1) * pageSize;
                var posts = postSearch.Skip(skipCount).Take(pageSize);
                ViewData["search"] = search;
                ViewData["totalPage"] = totalPage;
                ViewData["page"] = page;
                return View("Index", posts);
            }
        }
        
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id, string slug)
        {

            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategory
                .Include(p => p.BlogCategory)
                .Include(m => m.PostComments)
                .ThenInclude(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (postCategory == null)
            {
                return NotFound();
            }
            //var post = await _context.PostCategory.FindAsync(id);
            postCategory.ViewCount += 1;
            await _context.SaveChangesAsync();

            return View(postCategory);
        }

        // GET: PostCategories/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name");
            return View();
        }

        // POST: PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,CreateDate,UpdateDate,IsproductionReady,Slug,BlogCategoryId,ImageData")] PostCategory postCategory, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                
                postCategory.CreateDate = DateTime.Now;
                postCategory.UpdateDate = postCategory.CreateDate;
                postCategory.ViewCount = 0;

                postCategory.ContentType = _imageService.RecordContentType(image);
                postCategory.ImageData = await _imageService.EncodeFileAsync(image);


                var slug = _slugService.URLFriendly(postCategory.Title);
                if (_slugService.IsUnique(_context, slug))
                {
                    postCategory.Slug = slug;
                }
                else
                {
                    ModelState.AddModelError("Title", "This title cannot be used as it results in a duplicate Slug!");
                    ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", postCategory.BlogCategoryId);
                    return View(postCategory);
                }
                _context.Add(postCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", postCategory.BlogCategoryId);
            return View(postCategory);
        }

        // GET: PostCategories/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategory.FindAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", postCategory.BlogCategoryId);
            return View(postCategory);
        }

        // POST: PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,CreateDate,UpdateDate,IsproductionReady,Slug,BlogCategoryId,ImageData,ViewCount")] PostCategory postCategory, IFormFile image, Byte[] imageData, string contentType)
        {
            if (id != postCategory.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var slug = _slugService.URLFriendly(postCategory.Title);
                    if (image != null)
                    {
                        postCategory.ContentType = _imageService.RecordContentType(image);
                        postCategory.ImageData = await _imageService.EncodeFileAsync(image);
                    }
                    else
                    {
                        postCategory.ContentType = contentType;
                        postCategory.ImageData = imageData;
                    }
          
                    if (slug != postCategory.Slug)
                    {
                        if (_slugService.IsUnique(_context, slug))
                        {
                            postCategory.Slug = slug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This title cannot be used as it results in a duplicate Slug!");
                            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", postCategory.BlogCategoryId);
                            return View(postCategory);
                        }
                    }
                    postCategory.UpdateDate = DateTime.Now;
                    _context.Update(postCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "PostCategories", new { slug });
                    //return LocalRedirect($"/BlogPost/Details/{slug}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoryExists(postCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              
            }
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", postCategory.BlogCategoryId);
            return View(postCategory);
        }

        // GET: PostCategories/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategory
                .Include(p => p.BlogCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // POST: PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postCategory = await _context.PostCategory.FindAsync(id);
            _context.PostCategory.Remove(postCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategory.Any(e => e.Id == id);
        }


    }
}
