using AppleStore.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Admin + "," + Role.Role_Owner)]
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
