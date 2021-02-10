using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Display(Name = "Tag Name")]
        public string TagName { get; set; }

        public virtual ICollection<PostCategory> PostCategories { get; set; } = new HashSet<PostCategory>();
    }
}
