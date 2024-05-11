using AppleStore.Data;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AppleStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Role_Owner + "," + Role.Role_Admin)]
    public class AdminNewsOnTopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notyf;

        public AdminNewsOnTopController(ApplicationDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            var listNews = _context.NewsOnTops.ToList();
            return View(listNews);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(NewsOnTop newsOnTop, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                newsOnTop.Image = await CommonFunc.UploadFile(image, "news", CommonFunc.SEOUrl(newsOnTop.Header) + Path.GetExtension(image.FileName));

                _context.NewsOnTops.Add(newsOnTop);
                _context.SaveChanges();
                _notyf.Success("Tin nổi bật đã được tạo !");
                return RedirectToAction(nameof(Index));
            }
            return View(newsOnTop);
        }
        public IActionResult Edit(int id)
        {
            var newsOnTop = _context.NewsOnTops.FirstOrDefault(p => p.Id == id);
            return View(newsOnTop);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(NewsOnTop newsOnTop, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var existingNews = _context.NewsOnTops.FirstOrDefault(p => p.Id == newsOnTop.Id);
                
                if (image != null)
                {
                    newsOnTop.Image = await CommonFunc.UploadFile(image, "news", CommonFunc.SEOUrl(newsOnTop.Header) + Path.GetExtension(image.FileName));
                    existingNews.Image = newsOnTop.Image;
                }
                existingNews.Header = newsOnTop.Header;
                existingNews.SubHeader = newsOnTop.SubHeader;
                existingNews.Content = newsOnTop.Content;
                existingNews.Price = newsOnTop.Price;
                _context.NewsOnTops.Update(newsOnTop);
                _context.SaveChanges();
                _notyf.Success("Cập nhật tin nổi bật thành công !");
                return RedirectToAction("Index");
            }
            return View(newsOnTop);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var newsOnTop = _context.NewsOnTops.FirstOrDefault(p => p.Id == id);
            _context.NewsOnTops.Remove(newsOnTop);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
