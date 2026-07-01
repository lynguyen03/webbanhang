using System;

namespace _2380601309_NguyenHuuDieuLy_TH6.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Đang xử lý"; // Đang xử lý, Đang giao, Đã giao, Đã hủy
    }
}
