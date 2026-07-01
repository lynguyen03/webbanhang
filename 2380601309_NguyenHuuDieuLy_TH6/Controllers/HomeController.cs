using _2380601309_NguyenHuuDieuLy_TH6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _2380601309_NguyenHuuDieuLy_TH6.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
