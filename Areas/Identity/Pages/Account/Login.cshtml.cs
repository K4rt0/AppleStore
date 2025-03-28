﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppleStore.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using AppleStore.Data;
using AppleStore.Models.Entities.Google;

namespace AppleStore.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly INotyfService _notyf;
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, INotyfService notyf)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _notyf = notyf;
        }

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
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null, string access_token = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            if (!string.IsNullOrEmpty(access_token))
            {
                TokenGoogle data = await LoginGoogle.VerifyTokenGoogle(access_token);
                if (data.error_description == null)
                {
                    var user = await _userManager.FindByEmailAsync(data.email);
                    if (user == null)
                    {
                        var result = await _userManager.CreateAsync(new ApplicationUser { UserName = data.email, Email = data.email, FullName = data.name }); ;
                        if (result.Succeeded)
                            user = await _userManager.FindByEmailAsync(data.email);
                    }

                    if (user != null)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return LocalRedirect(returnUrl);
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (Input.Email == null || Input.Email == "")
            {
                _notyf.Warning("Email không được bỏ trống !");
                return Page();
            }
            if (Input.Password == null || Input.Password == "")
            {
                _notyf.Warning("Mật khẩu không được bỏ trống !");
                return Page();
            }
            var user = await _userManager.FindByNameAsync(Input.Email);
            if (user == null)
            {
                _notyf.Error("Người dùng này không tồn tại !");
                return Page();
            }
            if (user.EmailConfirmed == false)
            {
                _notyf.Error("Tài khoản của bạn chưa được xác thực !");
                return Page();
            }
            if (ModelState.IsValid)
            {
                if (!user.LockoutEnabled)
                {
                    _notyf.Error("Tài khoản của bạn đã bị vô hiệu hoá !");
                    _notyf.Warning("Vui lòng liên hệ karto.11203@gmail.com để được hỗ trợ", 5);
                    _notyf.GetNotifications();
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.Succeeded)
                {
                    _notyf.Success("Đăng nhập thành công !");
                    return LocalRedirect(returnUrl);
                }
                else if (!result.Succeeded)
                {
                    if (await CommonFunc.IsEmailExistsAsync(_context, Input.Email) == false)
                        _notyf.Error("Email của bạn không tồn tại trong hệ thống!");
                    else
                        _notyf.Error("Mật khẩu không hợp lệ !");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
