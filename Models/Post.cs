using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Content { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string ViewCount { get; set; }
        public string CommentCount { get; set; }
        public int Categoryid { get; set; }
        public string Category { get; set; }
    }
}
