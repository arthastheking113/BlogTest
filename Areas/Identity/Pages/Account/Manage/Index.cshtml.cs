﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BlogTest.Data;
using BlogTest.Models;
using BlogTest.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlogTest.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly SignInManager<BlogUser> _signInManager;
        private readonly IImageService _imageService;
        private readonly ApplicationDbContext _context;


        public IndexModel(ApplicationDbContext context,
            UserManager<BlogUser> userManager,
            SignInManager<BlogUser> signInManager, IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _imageService = imageService;
            _context = context;
        }
        [Display(Name = "User Name")]
        public string Username { get; set; }


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            [Display(Name = "First Name")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string FirstName { get; set; }


            [Display(Name = "Last Name")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string LastName { get; set; }

            [Display(Name = "Change Avatar")]
            public Byte[]? ImageData { get; set; }
            public string ContentType { get; set; }
        }

        private async Task LoadAsync(BlogUser user)
        {
            Username = await _userManager.GetUserNameAsync(user);

            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageData = user.ImageData,
                ContentType = user.ContentType
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile image,  Byte[]? imageData, string contentType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (image != null)
            {
                user.ImageData = await _imageService.EncodeFileAsync(image);
                user.ContentType = _imageService.RecordContentType(image);
            }
            else
            {
                if (imageData != null && contentType != null)
                {
                    user.ImageData = imageData;
                    user.ContentType = contentType;
                }
                else
                {
                    user.ImageData = null;
                    user.ContentType = null;
                }
               
            }
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
