using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogTest.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordNull : PageModel
    {
        public void OnGet()
        {
        }
    }
}
