using _2380601309_NguyenHuuDieuLy_TH6.Helpers;
using _2380601309_NguyenHuuDieuLy_TH6.Models;
using _2380601309_NguyenHuuDieuLy_TH6.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _2380601309_NguyenHuuDieuLy_TH6.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = ShoppingCartHelper.GetCart(HttpContext.Session);
            ViewBag.CartTotal = ShoppingCartHelper.GetCartTotal(HttpContext.Session);
            return View(cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity
            };

            ShoppingCartHelper.AddToCart(HttpContext.Session, cartItem);

            // Nếu request từ AJAX thì trả về JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cartCount = ShoppingCartHelper.GetCartCount(HttpContext.Session);
                return Json(new { success = true, cartCount = cartCount, message = "Đã thêm vào giỏ hàng!" });
            }

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            ShoppingCartHelper.RemoveFromCart(HttpContext.Session, productId);
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            ShoppingCartHelper.UpdateQuantity(HttpContext.Session, productId, quantity);
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        [HttpPost]
        public IActionResult Clear()
        {
            ShoppingCartHelper.ClearCart(HttpContext.Session);
            return RedirectToAction("Index");
        }

        // GET: Hiển thị form thanh toán
        public IActionResult Checkout()
        {
            var cart = ShoppingCartHelper.GetCart(HttpContext.Session);
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            ViewBag.CartTotal = ShoppingCartHelper.GetCartTotal(HttpContext.Session);
            return View();
        }

        // POST: Xử lý thanh toán
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            var cart = ShoppingCartHelper.GetCart(HttpContext.Session);
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // Set order metadata
                order.Id = _2380601309_NguyenHuuDieuLy_TH6.Areas.Admin.Controllers.OrderController.OrderList.Max(o => o.Id) + 1;
                order.OrderDate = DateTime.Now;
                order.TotalAmount = ShoppingCartHelper.GetCartTotal(HttpContext.Session);
                order.Status = "Đang xử lý";

                // Add to static list
                _2380601309_NguyenHuuDieuLy_TH6.Areas.Admin.Controllers.OrderController.OrderList.Add(order);

                // Clear cart
                ShoppingCartHelper.ClearCart(HttpContext.Session);

                return RedirectToAction("Success", new { id = order.Id });
            }

            ViewBag.CartTotal = ShoppingCartHelper.GetCartTotal(HttpContext.Session);
            return View(order);
        }

        // GET: Thành công
        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}
