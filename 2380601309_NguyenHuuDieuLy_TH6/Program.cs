using _2380601309_NguyenHuuDieuLy_TH6.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using _2380601309_NguyenHuuDieuLy_TH6.DataAccess;
using _2380601309_NguyenHuuDieuLy_TH6.Data;
using _2380601309_NguyenHuuDieuLy_TH6.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowOrigins", policy =>
    {
        //Thay bằng địa chỉ localhost khi khởi chạy bên frontend (VSCode)
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add session services for shopping cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Seed database with Roles and Admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Seed Roles
        string[] roleNames = { "Admin", "Customer" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Delete "User" role if it exists in the database
        var userRoleExist = await roleManager.RoleExistsAsync("User");
        if (userRoleExist)
        {
            var userRole = await roleManager.FindByNameAsync("User");
            if (userRole != null)
            {
                await roleManager.DeleteAsync(userRole);
            }
        }
        
        // Seed Categories and Products
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Văn học" },
                new Category { Name = "Kinh tế" },
                new Category { Name = "Tâm lý học" },
                new Category { Name = "Khoa học" },
                new Category { Name = "Tiểu thuyết" },
                new Category { Name = "Truyện tranh" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();
            
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Sapiens: Lược Sử Loài Người", Price = 155000, Description = "Khám phá lịch sử loài người từ thời đồ đá đến thế kỷ 21.", CategoryId = categories[3].Id, ImageUrl = "https://images.unsplash.com/photo-1544947950-fa07a98d237f?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "Đắc Nhân Tâm", Price = 85000, Description = "Nghệ thuật thu phục lòng người và giao tiếp hiệu quả.", CategoryId = categories[2].Id, ImageUrl = "https://images.unsplash.com/photo-1589829085413-56de8ae18c73?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "Nhà Giả Kim", Price = 75000, Description = "Hành trình theo đuổi ước mơ của cậu bé chăn cừu Santiago.", CategoryId = categories[4].Id, ImageUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "Tư Duy Nhanh Và Chậm", Price = 195000, Description = "Nghiên cứu về hai hệ thống tư duy chi phối quyết định của chúng ta.", CategoryId = categories[2].Id, ImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "Nghĩ Giàu Làm Giàu", Price = 110000, Description = "Bí quyết kinh doanh và làm giàu từ Napoleon Hill.", CategoryId = categories[1].Id, ImageUrl = "https://images.unsplash.com/photo-1554224155-8d04cb21cd6c?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "Vũ Trụ Của Carl Sagan", Price = 180000, Description = "Khám phá không gian bao la và những điều kỳ diệu của vũ trụ.", CategoryId = categories[3].Id, ImageUrl = "https://images.unsplash.com/photo-1614730321146-b6fa6a46bcb4?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "1984 - George Orwell", Price = 120000, Description = "Tiểu thuyết phản địa đàng kinh điển.", CategoryId = categories[4].Id, ImageUrl = "https://images.unsplash.com/photo-1519682337058-a94d519337bc?q=80&w=600&auto=format&fit=crop" },
                    new Product { Name = "Harry Potter và Hòn Đá Phù Thủy", Price = 165000, Description = "Cuộc phiêu lưu của cậu bé phù thủy Harry.", CategoryId = categories[4].Id, ImageUrl = "https://images.unsplash.com/photo-1618666012174-83b441c0bc76?q=80&w=600&auto=format&fit=crop" }
                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//Đặt trên UseAuthorization
app.UseCors("MyAllowOrigins");

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();
app.MapRazorPages();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

