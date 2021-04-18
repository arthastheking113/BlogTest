using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class SearchModel
    {
        public int Categoryids { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
    }
}
