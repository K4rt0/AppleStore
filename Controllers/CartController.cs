using AppleStore.Data;
using AppleStore.Extensions;
using AppleStore.Models;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace AppleStore.Controllers
{
    [Route("/Cart/")]
    [ApiController]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public INotyfService _notyf { get; }
        //private ReturnApi returnApi;
        public CartController(IProductRepository productRepository, INotyfService notyf,
            IProductVariantRepository productVariantRepository, IDiscountRepository discountRepository,
            ICartItemRepository cartItemRepository,
            UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _discountRepository = discountRepository;
            _cartItemRepository = cartItemRepository;
            _userManager = userManager;
            _notyf = notyf;
            _context = context;
            //returnApi = new();
        }

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return new ObjectResult(returnApi);
        //}


        //[HttpPost("{productId}")]
        //public async Task<IActionResult> AddToCart([FromRoute] string productId, [FromForm] string? quantity)
        //{
        //    await Task.CompletedTask;
        //    int _productId = Convert.ToInt32(productId);
        //    int _quantity = Convert.ToInt32(quantity);
        //    returnApi.success = true;
        //    returnApi.message = "null";
        //    returnApi.data = new { productId = _productId, quantity = _quantity };
        //    return new ObjectResult(returnApi);
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartItemRepository.GetAllByUserIdAsync(userId);

            return View(new ShoppingCart { Items = cartItems.ToList() });
        }

        [HttpPost("AddToCart/{productVariantId}")]
        public async Task<IActionResult> AddToCart(int productVariantId, [FromBody] int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var productVariant = await _productVariantRepository.GetByIdAsync(productVariantId);
            if (productVariant == null)
            {
                _notyf.Warning("Sản phẩm không tồn tại!");
                return Json(new { success = false });
            }

            var existingCartItem = await _cartItemRepository.GetByIdAndUserIdAsync(productVariantId, userId);

            if (existingCartItem != null)
            {
                if (existingCartItem.CartProductQuantity > 0)
                {
                    existingCartItem.CartProductQuantity += quantity;
                    await _cartItemRepository.UpdateAsync(existingCartItem);
                }
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductVariantId = productVariantId,
                    ProductVariant = productVariant,
                    CartProductQuantity = quantity,
                };
                await _cartItemRepository.AddAsync(cartItem);
            }

            _notyf.Success("Sản phẩm đã được thêm vào giỏ hàng.");
            return Json(new { success = true });
        }

        [HttpPost("UpdateCartItem/{productVariantId}")]
        public async Task<IActionResult> UpdateCartItem(int productVariantId,  string? quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemRepository.GetByIdAndUserIdAsync(productVariantId, userId);

            if (cartItem == null)
            {
                _notyf.Warning("Sản phẩm không tồn tại trong giỏ hàng!");
                return Json(new { success = false });
            }

            if (int.TryParse(quantity, out int _quantity) && _quantity < 1)
            {
                _notyf.Warning("Số lượng sản phẩm phải lớn hơn 0");
                return Json(new { success = false });
            }

            cartItem.CartProductQuantity = _quantity;
            await _cartItemRepository.UpdateAsync(cartItem);

            decimal itemTotalPrice = _quantity * cartItem.ProductVariant!.Price;
            var cartItems = await _cartItemRepository.GetAllByUserIdAsync(userId);
            var cartTotalPrice = cartItems.Sum(s => s.CartProductQuantity * s.ProductVariant!.Price);

            _notyf.Success("Cập nhật giỏ hàng thành công.");
            return Json(new
            {
                success = true,
                itemTotalPrice = itemTotalPrice.ToString("#,##0 đ"),
                cartTotalPrice = cartTotalPrice.ToString("#,##0 đ")
            });
        }

        [HttpPost("RemoveCartItem/{productVariantId}")]
        public async Task<IActionResult> RemoveCartItem(int productVariantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemRepository.GetByIdAndUserIdAsync(productVariantId, userId);

            if (cartItem == null)
            {
                _notyf.Warning("Sản phẩm không tồn tại trong giỏ hàng!");
                return Json(new { success = false });
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            _notyf.Success("Đã xóa sản phẩm khỏi giỏ hàng.");

            return Json(new { success = true });
        }
    }
}
