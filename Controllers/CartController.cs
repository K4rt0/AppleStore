using AppleStore.Data;
using AppleStore.Models;
using AppleStore.Models.Entities;
using AppleStore.Models.Entities.VNPay;
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

    [Authorize]
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IVnPayRespository _vnPayRespository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public INotyfService _notyf { get; }
        //private ReturnApi returnApi;
        public CartController(IProductRepository productRepository, INotyfService notyf,
            IProductVariantRepository productVariantRepository, IDiscountRepository discountRepository,
            ICartItemRepository cartItemRepository, IVnPayRespository vnPayRespository,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _vnPayRespository = vnPayRespository;
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
            ViewData["DbContext"] = _context;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int colorId, int storageId, int quantity)
        {
            var productVariantId = await _cartItemRepository.GetProductVariantByAttributeIdAsync(productId, colorId, storageId);
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
                    ApplicationUserId = userId,
                    ProductVariantId = productVariantId,
                    ProductVariant = productVariant,
                    CartProductQuantity = quantity,
                };
                await _cartItemRepository.AddAsync(cartItem);
            }

            _notyf.Success("Sản phẩm đã được thêm vào giỏ hàng.");
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int productVariantId, string? quantity)
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

        [HttpPost]
        public async Task<IActionResult> RemoveCartItem(int productVariantId, int amount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _cartItemRepository.GetByIdAndUserIdAsync(productVariantId, userId);

            if (cartItem == null)
            {
                _notyf.Warning("Sản phẩm không tồn tại trong giỏ hàng!");
                return Json(new { success = false });
            }
            _context.CartItems.Remove(cartItem);
            ViewData["ShoppingCart"] = new ShoppingCart { Items = (await _cartItemRepository.GetAllByUserIdAsync(userId)).ToList() };
            await _context.SaveChangesAsync();
            _notyf.Success("Đã xóa sản phẩm khỏi giỏ hàng.");

            return Json(new { success = true, amount = amount - 1 });
        }
        [HttpPost]
        public IActionResult CreateAddress(string userId, string fullName, int phoneNumber, string address)
        {
            DeliveryAddress deliveryAddress = new DeliveryAddress
            {
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Address = address,
                ApplicationUserId = userId
            };
            _context.Add(deliveryAddress);
            _context.SaveChanges();
            return Json(new
            {
                addressId = deliveryAddress.Id,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Address = address
            });
        }

        public async Task<IActionResult> GetCartItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartItemRepository.GetAllByUserIdAsync(userId);

            var cartItemsList = cartItems.Select(item => new
            {
                item.ProductVariant?.ProductId,
                Name = item.ProductVariant?.Product?.Name,
                Avatar = item.ProductVariant?.Product?.Avatar,
                CartProductQuantity = item.CartProductQuantity,
                Price = item.ProductVariant?.Price
            }).ToList();
            return Json(new
            {
                success = true,
                cartItemsList = cartItemsList
            });
        }
        public async Task<IActionResult> Checkout(int addressId)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["user"] = _context.ApplicationUsers?
                    .Include(p => p.CartItems)
                    .ThenInclude(p => p.ProductVariant)
                    .ThenInclude(p => p.VariantsAttributes)
                    .ThenInclude(p => p.ProductAttributeValue)
                    .ThenInclude(p => p.ProductAttribute)
                    .Include(p => p.CartItems)
                    .ThenInclude(p => p.ProductVariant)
                    .ThenInclude(p => p.Product)
                    .ThenInclude(p => p.Category)
                    .FirstOrDefault(u => u.Id == user.Id);

            ViewData["deliveryAddress"] = _context.DeliveryAddresses.FirstOrDefault(d => d.Id == addressId);
            return View();
        }
        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayRespository.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                _notyf.Error("Thanh toán thất bại!");
                return RedirectToAction("Index", "Home");
            }

            var order = _context.Orders.FirstOrDefault(p => p.Id == Convert.ToInt32(response.OrderDescription));

            order.Paid = true;
            _context.SaveChanges();

            _notyf.Success($"Hoá đơn #{order.Id} đã được tạo và thanh toán thành công!");
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public IActionResult CheckoutSubmit(int deliId, decimal amount, int payment)
        {
            if (ModelState.IsValid)
            {
                var deli = _context.DeliveryAddresses.Include(p => p.ApplicationUser).FirstOrDefault(d => d.Id == deliId);

                var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var cartItems = _context.CartItems
                    .Include(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                    .Where(ci => ci.ApplicationUserId == user)
                    .ToList();

                if (cartItems.Count == 0)
                {
                    _notyf.Error("Giỏ hàng của bạn trống!");
                    return RedirectToAction("Index");
                }

                var order = new Order
                {
                    CreatedAt = DateTime.Now,
                    Confirmed = false,
                    Paid = false,
                    PaymentMethod = payment == 1 ? "COD" : "VNPay",
                    TotalPrice = cartItems.Sum(ci => ci.CartProductQuantity * ci.ProductVariant.Price),
                    ApplicationUserId = user,
                    DeliveryAddressId = deliId,
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductVariant.ProductId,
                        Quantity = item.CartProductQuantity,
                        Price = item.ProductVariant.Price,
                    };
                    order.OrderDetails.Add(orderDetail);
                }

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();

                if (payment == 2)
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = amount,
                        CreatedDate = DateTime.Now,
                        Description = "",
                        FullName = deli.ApplicationUser.FullName,
                        AddressId = deliId,
                        OrderId = order.Id
                    };
                    return Redirect(_vnPayRespository.CreatePaymentUrl(HttpContext, vnPayModel));
                }
            }
            return View();
        }
    }
}