using AppleStore.Extensions;
using AppleStore.Models;
using AppleStore.Models.Entities;
using AppleStore.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Controllers
{
    [Route("/Cart/AddtoCart")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IDiscountRepository _discountRepository;
        public INotyfService _notyf { get; }
        //private ReturnApi returnApi;
        public CartController(IProductRepository productRepository, INotyfService notyf,
            IProductVariantRepository productVariantRepository, IDiscountRepository discountRepository)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _discountRepository = discountRepository;
            _notyf = notyf;
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

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        [Authorize]
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await GetProductFromDatabase(productId);
            product.Category.Products = null;

            var cartItem = new CartItem
            {
                ProductId = productId,
                Product = new Product
                {
                    Name = product.Name,
                    Category = product.Category,
                    Avatar = product.Avatar,
                },
                ProductVariant = new ProductVariant
                {
                    Id = 0,
                    Quantity = quantity,
                    Price = product.ProductVariants.FirstOrDefault()?.Price ?? 0,
                },
                Category = new Category
                {
                    Discount = product.Category.Discount,
                }
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return Json(new { success = true });
        }

        private async Task<Product> GetProductFromDatabase(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }
    }
}
