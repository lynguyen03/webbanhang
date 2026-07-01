using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace _2380601309_NguyenHuuDieuLy_TH6.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

