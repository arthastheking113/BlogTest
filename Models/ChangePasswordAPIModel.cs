using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest.Models
{
    public class ChangePasswordAPIModel
    {
        public string UserId { get; set; }
        public string Currentpassword { get; set; }
        public string Newpassword { get; set; }
    }
}
