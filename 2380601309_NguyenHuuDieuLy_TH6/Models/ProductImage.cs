namespace _2380601309_NguyenHuuDieuLy_TH6.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
        public Product? Product
        {
            get; set;
        }
    }
}
