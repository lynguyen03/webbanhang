using _2380601309_NguyenHuuDieuLy_TH6.Models;
using _2380601309_NguyenHuuDieuLy_TH6.Repositories;
using Microsoft.AspNetCore.Hosting; // Thêm thư viện này để dùng IWebHostEnvironment
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _2380601309_NguyenHuuDieuLy_TH6.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment; // Khai báo thêm biến môi trường

        // Inject thêm IWebHostEnvironment vào Constructor
        public ProductController(IProductRepository productRepository,
                               ICategoryRepository categoryRepository,
                               IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetProductsAsync();
            return View(products);
        }

        // Hiển thị form thêm sản phẩm mới
        [Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm mới
        [HttpPost]
        [Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Add(Product product, IFormFile ImageFile, List<IFormFile> ImageFiles)
        {
            foreach (var key in ModelState.Keys.ToList())
            {
                if (key != "Name" && key != "Price" && key != "Description" && key != "CategoryId")
                {
                    ModelState.Remove(key);
                }
            }

            if (ModelState.IsValid)
            {
                // Sử dụng WebRootPath giúp định vị chính xác thư mục wwwroot/images trên mọi ổ đĩa (C hoặc D)
                var imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                // Xử lý ảnh đại diện (ImageFile)
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(imagesFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    product.ImageUrl = "/images/" + fileName;
                }

                // Xử lý các ảnh phụ (ImageFiles)
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    if (product.Images == null)
                    {
                        product.Images = new List<ProductImage>();
                    }

                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(imagesFolder, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            product.Images.Add(new ProductImage { Url = "/images/" + fileName });
                        }
                    }
                }

                try
                {
                    await _productRepository.AddProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu database: " + ex.Message);
                }
            }

            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        [Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        [Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Update(int id, Product product, IFormFile ImageFile, List<IFormFile> ImageFiles)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            foreach (var key in ModelState.Keys.ToList())
            {
                if (key != "Id" && key != "Name" && key != "Price" && key != "Description" && key != "CategoryId")
                {
                    ModelState.Remove(key);
                }
            }

            if (ModelState.IsValid)
            {
                var imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                // Xử lý ảnh đại diện (ImageFile)
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(imagesFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    product.ImageUrl = "/images/" + fileName;
                }

                // Xử lý các ảnh phụ mới (ImageFiles)
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    if (product.Images == null)
                    {
                        product.Images = new List<ProductImage>();
                    }

                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(imagesFolder, fileName);
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            product.Images.Add(new ProductImage { Url = "/images/" + fileName });
                        }
                    }
                }

                try
                {
                    await _productRepository.UpdateProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi lưu database: " + ex.Message);
                }
            }

            // Nếu ModelState không hợp lệ, tải lại danh mục và ảnh phụ để hiển thị lại đúng cách
            var existingProduct = await _productRepository.GetProductByIdAsync(product.Id);
            if (existingProduct != null)
            {
                product.Images = existingProduct.Images;
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        [Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}