using BlogTest.Data;
using BlogTest.Models;
using BlogTest.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlogTest.Controllers.API
{
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly SignInManager<BlogUser> _signInManager;
        private readonly IJWTTokenGenerator _jwtToken;
        private readonly ILogger<RegisterAPIModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public IdentityController(UserManager<BlogUser> userManager,
            SignInManager<BlogUser> signInManager,
             IJWTTokenGenerator jwtToken,
             ILogger<RegisterAPIModel> logger,
             IEmailSender emailSender,
             IConfiguration config,
             ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtToken = jwtToken;
            _logger = logger;
            _emailSender = emailSender;
            _config = config;
            _context = context;
        }
        [HttpPost("getinfor")]
        public async Task<IActionResult> GetinformationAsync(GetuserInformationAPIModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return BadRequest("User is not exsit");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginAPIModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return BadRequest("Incorrect User Name or Password");
            }
            if (!user.EmailConfirmed)
            {
                return BadRequest("Please confirm your email");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest("Incorrect User Name or Password");
            }
            var roles = await _userManager.GetRolesAsync(user);
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            var token = _jwtToken.GenerateToken(user, roles, claims);
            return Ok(new
            {
                result = result,
                username = user.Email,
                email = user.Email,
                firstName = user.FirstName.ToString(),
                lastName = user.LastName.ToString(),
                token = token,
                userid = user.Id
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterAPIModel model)
        {
            if (model.Password == model.Confirmpassword)
            {
                var newuser = new BlogUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                var result = await _userManager.CreateAsync(newuser, model.Password);

                if (result.Succeeded)
                {
                    var userFromDb = await _userManager.FindByEmailAsync(newuser.UserName);

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(userFromDb);

                    var uriBuilder = new UriBuilder(_config["ReturnPaths:confirmemail"]);
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["token"] = token;
                    query["userid"] = userFromDb.Id;
                    uriBuilder.Query = query.ToString();
                    var urlString = uriBuilder.ToString();



                    await _emailSender.SendEmailAsync(userFromDb.Email, "Confirm your email address",
                        $"Please confirm your account by <a href='{urlString}'>clicking here</a>.");

                    //Add role to user
                    //await _userManager.AddToRoleAsync(userFromDb, model.Role);

                    //var claim = new Claim("JobTitle", model.JobTitle);

                    //await _userManager.AddClaimAsync(userFromDb, claim);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest();
        }
        [HttpPost("resentemailconfirmation")]
        public async Task<IActionResult> ResentEmailConfirmationAsync(ResendEmailConfirmationAPIModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return BadRequest("Invalid Email");
            }
            if (user.EmailConfirmed)
            {
                return BadRequest("Your account is comfirmed, you can login now");
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var uriBuilder = new UriBuilder(_config["ReturnPaths:confirmemail"]);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            query["userid"] = user.Id;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email address",
                $"Please confirm your account by <a href='{urlString}'>clicking here</a>.");
            return Ok();
        }

        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailAPIModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            user.EmailConfirmed = true;
            _context.Update(user);
            await _context.SaveChangesAsync();
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordAPIModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var uriBuilder = new UriBuilder(_config["ReturnPaths:resetpassword"]);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                uriBuilder.Query = query.ToString();
                var urlString = uriBuilder.ToString();

                await _emailSender.SendEmailAsync(user.Email, "Reset your password",
                    $"Please <a href='{urlString}'>clicking here</a> to reset your account's password.");

                return Ok(new
                {
                    userid = user.Id,
                    token = token
                });
            }
            return BadRequest("Email is not valid");
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetpasswordAsync(ResetPasswordAPIModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is not null)
            {
                model.Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                return BadRequest("Invalid token");
            }
            return BadRequest("Email is not valid");
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangepasswordAsync(ChangePasswordAPIModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return BadRequest("User is not exsit");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Currentpassword, model.Newpassword);
            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(changePasswordResult.Errors);
            }
            return Ok();
        }

        [HttpPost("changeemail")]
        public async Task<IActionResult> ChangeemailAsync(ChangeEmailAPIModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return BadRequest("User is not exsit");
            }
            if (_context.Users.FirstOrDefault(u => u.Email == model.NewEmail) is not null)
            {
                return BadRequest("You can't use this email. Email is already exsit");
            }
            var newEmail = model.NewEmail;
            user.EmailConfirmed = false;
            user.UserName = newEmail;
            user.Email = newEmail;
            user.NormalizedEmail = newEmail;
            user.NormalizedUserName = newEmail;
            _context.Update(user);
            await _context.SaveChangesAsync();
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var uriBuilder = new UriBuilder(_config["ReturnPaths:changeemailsuccess"]);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            query["userid"] = user.Id;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();

            await _emailSender.SendEmailAsync(newEmail, "Confirm your email address",
                $"Please confirm your account by <a href='{urlString}'>clicking here</a>.");

            return Ok();
        }


        [HttpPost("changeusername")]
        public async Task<IActionResult> ChangeusernameAsync(ChangeUserNameAPIModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                return BadRequest("User is not exsit");
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
