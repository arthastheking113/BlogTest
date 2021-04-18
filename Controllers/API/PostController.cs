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
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;

        public PostController(ApplicationDbContext context,
            IImageService imageService)
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
        [HttpPost("getpost")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostAsync(SearchModel model)
        {
            if (model.TotalPage == 0)
            {
                model.TotalPage = 1;
            }
            if (model.CurrentPage == 0)
            {
                model.CurrentPage = 1;
            }
            var pageSize = 9;
            var totalPage = 0;
            var totalRecord = 0;
            if (model.Categoryids != 0)
            {
                var postIdSearch = _context.PostCategory.Where(h => h.BlogCategoryId == model.Categoryids);
                if (!string.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower();
                    var postSearch = await postIdSearch.Include(h => h.BlogCategory)
                        .Include(h => h.PostComments).Where(y => y.IsproductionReady)
                        .Where(x => x.Title.ToLower().Contains(model.Search) ||
                                                                    x.Content.ToLower().Contains(model.Search) ||
                                                                    x.Abstract.ToLower().Contains(model.Search) ||
                                                                    x.BlogCategory.Name.ToLower().Contains(model.Search) ||
                                                                    x.BlogCategory.Description.ToLower().Contains(model.Search) ||
                                                                    x.PostComments.Any(c => c.Content.ToLower().Contains(model.Search) ||
                                                                                        c.BlogUser.FirstName.ToLower().Contains(model.Search) ||
                                                                                        c.BlogUser.LastName.ToLower().Contains(model.Search) ||
                                                                                        c.BlogUser.Email.ToLower().Contains(model.Search))).ToListAsync();
                    postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                    totalRecord = postSearch.Count;
                    totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                    var skipCount = ((int)model.TotalPage - 1) * pageSize;
                    var posts = postSearch.Skip(skipCount).Take(pageSize);
                    List<Post> listPost = new List<Post>();
                    foreach (var item in posts)
                    {
                        var imageData = _imageService.DecodeFile(item.ImageData, item.ContentType);
                        var post = new Post
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Abstract = StripHTML(item.Abstract),
                            Content = Regex.Replace(item.Content, @"<(.|\n)*?>", string.Empty),
                            CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            Slug = item.Slug,
                            Image = imageData,
                            ViewCount = item.ViewCount.ToString(),
                            CommentCount = item.PostComments.Count.ToString(),
                            Categoryid = item.BlogCategoryId,
                            Category = item.BlogCategory.Name
                        };
                        listPost.Add(post);
                    }
                    return Ok(listPost);

                }
                else
                {
                    var postSearch = await postIdSearch.Include(h => h.PostComments).Include(p => p.BlogCategory).Where(y => y.IsproductionReady).ToListAsync();
                    postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                    totalRecord = postSearch.Count;
                    totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                    var skipCount = ((int)model.TotalPage - 1) * pageSize;
                    var posts = postSearch.Skip(skipCount).Take(pageSize);
                    List<Post> listPost = new List<Post>();
                    foreach (var item in posts)
                    {
                        var imageData = _imageService.DecodeFile(item.ImageData, item.ContentType);
                        var post = new Post
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Abstract = StripHTML(item.Abstract),
                            Content = Regex.Replace(item.Content, @"<(.|\n)*?>", string.Empty),
                            CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            Slug = item.Slug,
                            Image = imageData,
                            ViewCount = item.ViewCount.ToString(),
                            CommentCount = item.PostComments.Count.ToString(),
                            Categoryid = item.BlogCategoryId,
                            Category = item.BlogCategory.Name
                        };
                        listPost.Add(post);
                    }
                    return Ok(listPost);
                }
            }
            else
            {
                var postIdSearch = _context.PostCategory;
                if (model.Search != null)
                {
                    model.Search = model.Search.ToLower();
                    var postSearch = await postIdSearch.Include(h => h.BlogCategory)
                        .Include(h => h.PostComments).Where(y => y.IsproductionReady)
                        .Where(x => x.Title.ToLower().Contains(model.Search) ||
                                                                    x.Content.ToLower().Contains(model.Search) ||
                                                                    x.Abstract.ToLower().Contains(model.Search) ||
                                                                    x.BlogCategory.Name.ToLower().Contains(model.Search) ||
                                                                    x.BlogCategory.Description.ToLower().Contains(model.Search) ||
                                                                    x.PostComments.Any(c => c.Content.ToLower().Contains(model.Search) ||
                                                                                        c.BlogUser.FirstName.ToLower().Contains(model.Search) ||
                                                                                        c.BlogUser.LastName.ToLower().Contains(model.Search) ||
                                                                                        c.BlogUser.Email.ToLower().Contains(model.Search))).ToListAsync();
                    postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                    totalRecord = postSearch.Count;
                    totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                    var skipCount = ((int)model.TotalPage - 1) * pageSize;
                    var posts = postSearch.Skip(skipCount).Take(pageSize);
                    List<Post> listPost = new List<Post>();
                    foreach (var item in posts)
                    {
                        var imageData = _imageService.DecodeFile(item.ImageData, item.ContentType);
                        var post = new Post
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Abstract = StripHTML(item.Abstract),
                            Content = Regex.Replace(item.Content, @"<(.|\n)*?>", string.Empty),
                            CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            Slug = item.Slug,
                            Image = imageData,
                            ViewCount = item.ViewCount.ToString(),
                            CommentCount = item.PostComments.Count.ToString(),
                            Categoryid = item.BlogCategoryId,
                            Category = item.BlogCategory.Name
                        };
                        listPost.Add(post);
                    }
                    return Ok(listPost);

                }
                else
                {
                    var postSearch = await postIdSearch.Include(h => h.PostComments)
                        .Include(p => p.BlogCategory).Where(y => y.IsproductionReady).ToListAsync();
                    postSearch = postSearch.OrderByDescending(c => c.UpdateDate).ToList();
                    totalRecord = postSearch.Count;
                    totalPage = Convert.ToInt32(totalRecord / pageSize) + 1;
                    var skipCount = ((int)model.TotalPage - 1) * pageSize;
                    var posts = postSearch.Skip(skipCount).Take(pageSize);
                    List<Post> listPost = new List<Post>();
                    foreach (var item in posts)
                    {
                        var imageData = _imageService.DecodeFile(item.ImageData, item.ContentType);
                        var post = new Post
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Abstract = StripHTML(item.Abstract),
                            Content = Regex.Replace(item.Content, @"<(.|\n)*?>", string.Empty),
                            CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                            Slug = item.Slug,
                            Image = imageData,
                            ViewCount = item.ViewCount.ToString(),
                            CommentCount = item.PostComments.Count.ToString(),
                            Categoryid = item.BlogCategoryId,
                            Category = item.BlogCategory.Name
                        };
                        listPost.Add(post);
                    }
                    return Ok(listPost);
                }
            }

           
           
        }

        [HttpGet("getRandom4Posts")]
        public async Task<ActionResult<IEnumerable<Post>>> GetRandom4PostsAsync()
        {
            var count = 1;
            var postList = _context.PostCategory.Include(h => h.BlogCategory).Include(h => h.PostComments).ToList();
            if (postList.Count > 4)
            {
                postList = postList.OrderBy(a => Guid.NewGuid()).ToList();
                List<Post> Posts = new List<Post>();
                foreach (var item in postList)
                {
                    var imageData = _imageService.DecodeFile(item.ImageData, item.ContentType);
                    var post = new Post
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Abstract = Regex.Replace(item.Abstract, @"<(.|\n)*?>", string.Empty),
                        Content = Regex.Replace(item.Content, @"<(.|\n)*?>", string.Empty),
                        CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                        UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                        Slug = item.Slug,
                        Image = imageData,
                        ViewCount = item.ViewCount.ToString(),
                        CommentCount = item.PostComments.Count.ToString(),
                        Categoryid = item.BlogCategoryId,
                        Category = item.BlogCategory.Name
                    };
                    Posts.Add(post);
                    if (count == 4)
                    {
                        break;
                    }
                    else
                    {

                        count += 1;
                    }
                }
                return Ok(Posts);
            }
            else
            {
                List<Post> Posts = new List<Post>();
                foreach (var item in postList)
                {
                    var imageData = _imageService.DecodeFile(item.ImageData, item.ContentType);
                    var post = new Post
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Abstract = Regex.Replace(item.Abstract, @"<(.|\n)*?>", string.Empty),
                        Content = Regex.Replace(item.Content, @"<(.|\n)*?>", string.Empty),
                        CreateDate = item.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                        UpdateDate = item.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                        Slug = item.Slug,
                        Image = imageData,
                        ViewCount = item.ViewCount.ToString(),
                        CommentCount = item.PostComments.Count.ToString(),
                        Categoryid = item.BlogCategoryId,
                        Category = item.BlogCategory.Name
                    };
                    Posts.Add(post);
                }
                return Ok(Posts);
            }
        }


        [HttpGet("postdetails/{slug}")]
        public async Task<IActionResult> getPostAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest("Invalid Slug");
            }

            var postCategory = await _context.PostCategory
                .Include(p => p.BlogCategory)
                .Include(m => m.PostComments)
                .ThenInclude(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (postCategory == null)
            {
                return BadRequest("Undefined post");
            }
            postCategory.ViewCount += 1;
            await _context.SaveChangesAsync();
            var imageData = _imageService.DecodeFile(postCategory.ImageData, postCategory.ContentType);
            Post post = new Post
            {
                Id = postCategory.Id,
                Title = postCategory.Title,
                Abstract = Regex.Replace(postCategory.Abstract, @"<(.|\n)*?>", string.Empty),
                Content = "<style> img { width: 100 %; height: auto;} </style>" + postCategory.Content,
                CreateDate = postCategory.CreateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                UpdateDate = postCategory.UpdateDate.ToString("dd MMMM yyyy - hh:mm tt"),
                Slug = postCategory.Slug,
                Image = imageData,
                ViewCount = postCategory.ViewCount.ToString(),
                CommentCount = postCategory.PostComments.Count.ToString(),
                Categoryid = postCategory.BlogCategoryId,
                Category = postCategory.BlogCategory.Name
            };
            return Ok(post);
        }
    }
}
