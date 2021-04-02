using BlogTest.Data;
using BlogTest.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PagedList;
using PagedList.Core.Mvc;

namespace BlogTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<HomeController> _logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Name")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Subject")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string Subject { get; set; }

            [Required]
            [Display(Name = "Your Message")]
            [StringLength(3000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 10)]
            public string Message { get; set; }
        }

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page,string search)
        {
            page ??= 1;
            var pageSize = 9;
            var totalPage = 0;
            var totalRecord = 0;

            if (search != null)
            {
                search = search.ToLower();
                var postSearch = await _context.PostCategory.Include(h => h.PostComments).Where(y => y.IsproductionReady).Where(x => x.Title.ToLower().Contains(search) ||
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
                return View(posts);
            }
            else
            {   
                var postSearch = await _context.PostCategory.Include(h => h.PostComments).Include(p => p.BlogCategory).Where(y => y.IsproductionReady).ToListAsync();
                postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                totalRecord = postSearch.Count;
                totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                var skipCount = ((int)page - 1) * pageSize;
                var posts = postSearch.Skip(skipCount).Take(pageSize);
                ViewData["search"] = search;
                ViewData["totalPage"] = totalPage;
                ViewData["page"] = page;
                return View(posts);
            }
        }

        

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
     
        [HttpPost]
        public async Task<IActionResult> Contact(string Name, string Email, string Subject, string Message)
        {
            string myEmail = "arthastheking113@gmail.com";

            string subject = $"{Name} Just Send You A Message About {Subject}";

            string body = $"Message From {Email}: {Message}";

            string customberSubject = $"I Just Received A Message From Lan's Blog With Name {Name} About {Subject}";
            string customberBody = $"I Received Message From {Email} About: {Subject}. I will contact back to you as soon as possible.";

            await _emailSender.SendEmailAsync(myEmail, subject, body);

            await _emailSender.SendEmailAsync(Email, customberSubject, customberBody);

            ModelState.Clear();
            return View("SendMessageSuccess");
        }

        public IActionResult SendMessageSuccess()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
