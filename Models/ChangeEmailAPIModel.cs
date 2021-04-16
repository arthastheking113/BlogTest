using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class ChangeEmailAPIModel
    {
        public string UserId { get; set; }
        public string NewEmail { get; set; }
    }
}
