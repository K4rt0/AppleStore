using AppleStore.Data;
using AppleStore.Models.Entities;
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

        public AdminUserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = Role.Role_Owner)]
        public IActionResult ListAdmin()
        {
            var users = _context.ApplicationUsers.ToList(); 
            return View(users);
        }
        public IActionResult ListUser()
        {
            var users = _context.ApplicationUsers.ToList();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> TakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (await _userManager.IsInRoleAsync(user, Role.Role_Admin))
            {
                await _userManager.RemoveFromRoleAsync(user, Role.Role_Admin);
            }

            if (!await _userManager.IsInRoleAsync(user, Role.Role_Customer))
            {
                var result = await _userManager.AddToRoleAsync(user, Role.Role_Customer);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.AddToRoleAsync(user, Role.Role_Admin);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
