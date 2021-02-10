using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogTest.Data;
using BlogTest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogTest.Controllers
{
    public class PostCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly SignInManager<BlogUser> _signInManager;

        public PostCommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager, SignInManager<BlogUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: PostComments
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PostComment.Include(p => p.PostCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PostComments/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postComment = await _context.PostComment
                .Include(p => p.PostCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postComment == null)
            {
                return NotFound();
            }

            return View(postComment);
        }

        // GET: PostComments/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategory, "Id", "Id");
            return View();
        }

        // POST: PostComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,CommentDate,Update,PostCategoryId,BlogUserId")] PostComment postComment, string Slug)
        {
            if (ModelState.IsValid)
            {
                postComment.CommentDate = DateTime.Now;
                postComment.Update = postComment.CommentDate;
                postComment.BlogUserId = _userManager.GetUserId(User);
                _context.Add(postComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","PostCategories", new { Slug });
            }
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategory, "Id", "Id", postComment.PostCategoryId);
            return View(postComment);
        }

        // GET: PostComments/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postComment = await _context.PostComment.FindAsync(id);
            if (postComment == null)
            {
                return NotFound();
            }
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategory, "Id", "Id", postComment.PostCategoryId);
            return View(postComment);
        }

        // POST: PostComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CommentDate,Update,Moderated,ModeratedReason,ModeratedContent,PostCategoryId,BlogUserId")] PostComment postComment)
        {
            if (id != postComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCommentExists(postComment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategory, "Id", "Id", postComment.PostCategoryId);
            return View(postComment);
        }

        // GET: PostComments/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postComment = await _context.PostComment
                .Include(p => p.PostCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postComment == null)
            {
                return NotFound();
            }

            return View(postComment);
        }

        // POST: PostComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var postComment = await _context.PostComment.FindAsync(id);
            _context.PostComment.Remove(postComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostCommentExists(int id)
        {
            return _context.PostComment.Any(e => e.Id == id);
        }
    }
}
