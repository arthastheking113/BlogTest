﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class PostComment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [Display(Name = "Comment Date")]
        public DateTime CommentDate { get; set; }
        public DateTime? Update { get; set; }

        public DateTime? Moderated { get; set; }
        [Display(Name ="Moderated Reason")]
        public string ModeratedReason { get; set; }
        [Display(Name = "Moderated Content")]
        public string ModeratedContent { get; set; }

        [Display(Name = "Category")]
        public int PostCategoryId { get; set; }
        public virtual PostCategory PostCategory { get; set; }

        [Display(Name = "User")]
        public string BlogUserId { get; set; }
        public virtual BlogUser BlogUser { get; set; }
    }
}
