using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class CommentAPIModel
    {
        public string Slug { get; set; }
        public string Userid { get; set; }
        public string Content { get; set; }
    }
}
