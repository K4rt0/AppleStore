// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AppleStore.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppleStore.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notyf;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            INotyfService notyf)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf = notyf;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Full Name")]
            public string Fullname { get; set; }

            [DisplayName("Ngày sinh")]
            public DateOnly Birthdate { get; set; }

            [DisplayName("Địa chỉ")]
            public string? Address { get; set; }

            [DisplayName("Giới tính")]
            public bool Gender { get; set; }
            [DisplayName("Ảnh đại diện")]
            public string? Avatar { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var fullName = user.FullName;
            var Birthdate = user.Birthdate;
            var Address = user.Address;
            var gender = user.Gender;
            var Avatar = user.Avatar;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Fullname = fullName,
                Birthdate = Birthdate,
                Address = Address,
                Gender = gender,
                Avatar = Avatar,
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

        public async Task SaveImageToDatabaseAsync(string imagePath, ApplicationUser user)
        {
            user.Avatar = imagePath;
            await _userManager.UpdateAsync(user);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            else
            {
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set phone number.";
                        return RedirectToPage();
                    }
                }

                if (Input.Fullname != user.FullName || Input.Birthdate != user.Birthdate ||
                    Input.Address != user.Address || Input.Gender != user.Gender)
                {
                    user.FullName = Input.Fullname;
                    user.Birthdate = Input.Birthdate;
                    user.Address = Input.Address;
                    user.Gender = Input.Gender;
                    await _userManager.UpdateAsync(user);
                }

                //// Kiểm tra xem người dùng đã tải lên ảnh mới chưa
                //if (Input.Avatar != user.Avatar)
                //{
                //    if (Request.Form.Files.Count > 0)
                //    {
                //        var file = Request.Form.Files[0];
                //        if (file != null && file.Length > 0)
                //        {
                //            // Lưu ảnh vào thư mục hoặc lưu trữ ảnh
                //            var imagePath = SaveImageToFileSystem(file);

                //            // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                //            await SaveImageToDatabaseAsync(imagePath, user);
                //        }
                //    }
                //}

                if(!string.IsNullOrEmpty(Input.Avatar))
                {
                    user.Avatar = Input.Avatar;
                    await _userManager.UpdateAsync(user);
                }

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Thông tin của bạn đã được cập nhật.";
                return RedirectToPage();
            }
        }
        private string SaveImageToFileSystem(IFormFile file)
        {
            // Đường dẫn tạm thời để lưu ảnh
            var imagePath = Path.Combine("wwwroot", "images", "team", file.FileName);

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return imagePath;
        }
    }
}
