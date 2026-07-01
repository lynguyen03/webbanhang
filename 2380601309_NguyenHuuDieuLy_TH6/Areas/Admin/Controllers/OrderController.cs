using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _2380601309_NguyenHuuDieuLy_TH6.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace _2380601309_NguyenHuuDieuLy_TH6.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,admin")]
    public class OrderController : Controller
    {
        public static List<Order> OrderList = new List<Order>
        {
            new Order { Id = 1001, CustomerName = "Nguyễn Văn A", ShippingAddress = "123 Đường Lê Lợi, Quận 1, TP.HCM", PhoneNumber = "0901234567", OrderDate = DateTime.Now.AddDays(-3), TotalAmount = 1500000, Status = "Đã giao" },
            new Order { Id = 1002, CustomerName = "Trần Thị B", ShippingAddress = "456 Đường Nguyễn Huệ, Quận 3, TP.HCM", PhoneNumber = "0912345678", OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 2300000, Status = "Đang giao" },
            new Order { Id = 1003, CustomerName = "Lê Hoàng C", ShippingAddress = "789 Đường Điện Biên Phủ, Bình Thạnh, TP.HCM", PhoneNumber = "0987654321", OrderDate = DateTime.Now.AddDays(-1), TotalAmount = 850000, Status = "Đang xử lý" },
            new Order { Id = 1004, CustomerName = "Phạm Minh D", ShippingAddress = "101 Đường Cách Mạng Tháng 8, Quận 10, TP.HCM", PhoneNumber = "0934567890", OrderDate = DateTime.Now, TotalAmount = 4200000, Status = "Đang xử lý" }
        };

        // GET: Admin/Order
        public IActionResult Index()
        {
            return View(OrderList);
        }

        // GET: Admin/Order/Details/5
        public IActionResult Details(int id)
        {
            var order = OrderList.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: Admin/Order/Edit/5
        public IActionResult Edit(int id)
        {
            var order = OrderList.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string status)
        {
            var order = OrderList.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            order.Status = status;
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Order/Delete/5
        public IActionResult Delete(int id)
        {
            var order = OrderList.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Admin/Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = OrderList.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                OrderList.Remove(order);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
