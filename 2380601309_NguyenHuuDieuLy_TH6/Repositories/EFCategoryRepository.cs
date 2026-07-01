using _2380601309_NguyenHuuDieuLy_TH6.DataAccess;
using _2380601309_NguyenHuuDieuLy_TH6.Models;
using Microsoft.EntityFrameworkCore;
namespace _2380601309_NguyenHuuDieuLy_TH6.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy toàn bộ danh sách Category
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // 2. Tìm một Category theo Id
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // 3. Thêm một Category mới
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        // 4. Cập nhật thông tin Category
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // 5. Xóa một Category theo Id
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}