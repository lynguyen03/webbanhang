using _2380601309_NguyenHuuDieuLy_TH6.Models;

namespace _2380601309_NguyenHuuDieuLy_TH6.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
