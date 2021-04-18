using BlogTest.Data;
using BlogTest.Models;
using BlogTest.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BlogTest.Controllers.API
{
    [Route("[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public CommentController(ApplicationDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }
        public static string StripHTML(string HTMLText, bool decode = true)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var stripped = reg.Replace(HTMLText, "");
            return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
        }
        [HttpGet("getComment/{slug}")]
        public async Task<ActionResult<IEnumerable<Comment>>> getCommentAsync(string slug)
       {
            var categoryId = _context.PostCategory.FirstOrDefault(h => h.Slug == slug).Id;
            var comments = _context.PostComment.Include(c => c.BlogUser).Include(c => c.PostCategory).Where(c => c.PostCategoryId == categoryId).ToList();
            List<Comment> listComment = new List<Comment>();
            foreach (var item in comments)
            {
                var imageData = _imageService.DecodeFile(item.BlogUser.ImageData, item.BlogUser.ContentType);
                var currentTime = DateTime.Now;
                if (!item.IsModerated)
                {
                    var timeSpan = currentTime.Subtract(item.CommentDate);
                    var eslapseSecond = timeSpan.Seconds;
                    var eslapseMinute = timeSpan.Minutes;
                    var eslapseHours = timeSpan.Hours;
                    var eslapseDate = timeSpan.Days;
                    string commentdate;
                    if (eslapseDate >= 1)
                    {
                        commentdate = eslapseDate.ToString() + " Days Ago";

                    }
                    else if (eslapseHours >= 1)
                    {
                        commentdate = eslapseHours.ToString() + " Hours Ago";
                    }
                    else if (eslapseMinute >= 1)
                    {
                        commentdate = eslapseMinute.ToString() + " Minutes Ago";
                    }
                    else
                    {
                        commentdate = eslapseSecond.ToString() + " Seconds Ago";
                    }
                    Comment comment = new Comment
                    {
                        Id = item.Id,
                        Name = item.BlogUser.FullName,
                        Image = imageData,
                        Content = item.Content,
                        Date = commentdate
                    };
                    listComment.Add(comment);  
                }
                else
                {
                    var timeSpan = currentTime.Subtract(item.Moderated);
                    var eslapseSecond = timeSpan.Seconds;
                    var eslapseMinute = timeSpan.Minutes;
                    var eslapseHours = timeSpan.Hours;
                    var eslapseDate = timeSpan.Days;
                    string commentdate;
                    if (eslapseDate >= 1)
                    {
                        commentdate = eslapseDate.ToString() + " Days Ago";

                    }
                    else if (eslapseHours >= 1)
                    {
                        commentdate = eslapseHours.ToString() + " Hours Ago";
                    }
                    else if (eslapseMinute >= 1)
                    {
                        commentdate = eslapseMinute.ToString() + " Minutes Ago";
                    }
                    else
                    {
                        commentdate = eslapseSecond.ToString() + " Seconds Ago";
                    }
                    Comment comment = new Comment
                    {
                        Id = item.Id,
                        Name = item.BlogUser.FullName,
                        Image = imageData,
                        Content = item.Content,
                        Date = commentdate
                    };
                    listComment.Add(comment);
                }
            }
            return Ok(listComment);
        }
    }
}
