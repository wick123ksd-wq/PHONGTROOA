using Microsoft.AspNetCore.Mvc;
using PHONGTROOA.Data;
using PHONGTROOA.Models;
using System.Diagnostics;

namespace PHONGTROOA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rooms = _context.Phongs.ToList();
            return View(rooms);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}