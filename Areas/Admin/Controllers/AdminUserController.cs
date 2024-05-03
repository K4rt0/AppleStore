using AppleStore.Data;
using AppleStore.Models.Entities;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Admin + "," + Role.Role_Owner)]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotyfService _notyf;

        public AdminUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INotyfService notyf)
        {
            _context = context;
            _userManager = userManager;
            _notyf = notyf;
        }
        [Authorize(Roles = Role.Role_Owner)]
        public IActionResult ListAdmin()
        {
            var users = _context.ApplicationUsers.Where(p => p.LockoutEnabled == true).ToList();
            return View(users);
        }
        public IActionResult ListUser()
        {
            var users = _context.ApplicationUsers.Where(p => p.LockoutEnabled == true).ToList();
            return View(users);
        }
        public IActionResult ListUserBlocked()
        {
            var users = _context.ApplicationUsers.Where(p => p.LockoutEnabled == false).ToList();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> AdminPanel(string type, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            IdentityResult result = new IdentityResult();
            if(type == "Make")
            {
                result = await _userManager.AddToRoleAsync(user, Role.Role_Admin);
                if (!result.Succeeded)
                    return BadRequest();
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, Role.Role_Customer);
                    _notyf.Information($"Bạn đã cấp quyền Administrator cho {user.FullName} !");
                }
            }
            else if (type == "Take")
            {
                result = await _userManager.RemoveFromRoleAsync(user, Role.Role_Admin);
                if (!result.Succeeded)
                    return BadRequest();
                else
                {
                    await _userManager.AddToRoleAsync(user, Role.Role_Customer);
                    _notyf.Information($"Bạn đã thu hồi quyền Administrator của {user.FullName} !");
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public async Task<IActionResult> LockUser(string id, bool lockout)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            SetUserLockout(user, lockout);
            if(lockout)
                _notyf.Success($"Bạn đã mở khoá tài khoản của {user.FullName} !");
            else
                _notyf.Error($"Bạn đã khoá tài khoản của {user.FullName} !");

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public void SetUserLockout(ApplicationUser user, bool lockout)
        {
            user.LockoutEnabled = lockout;
            _context.SaveChanges();
        }
    }
}
