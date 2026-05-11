using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Data;
using PHONGTROOA.Models;
using System.Security.Claims;

namespace PHONGTROOA.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= ADD TO CART =================
        [Authorize]
        public IActionResult AddToCart(int id, int months, DateTime startDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (months <= 0)
            {
                return Content("Số tháng phải lớn hơn 0");
            }

            var phong = _context.Phongs.FirstOrDefault(p => p.MaPhong == id);
            if (phong == null)
            {
                return Content("Phòng không tồn tại");
            }

            var endDate = startDate.AddMonths(months);

            // 🔥 กันซ้ำตั้งแต่แรก (LOCK)
            var isBooked = _context.Bookings.Any(b =>
                b.PhongId == id &&
                b.Status != "Cancelled" &&
                b.StartDate < endDate &&
                b.EndDate > startDate
            );

            if (isBooked)
            {
                TempData["Error"] = "❌ ห้องนี้ถูกจองแล้วในช่วงเวลานี้!";
                return RedirectToAction("ChiTiet", "Room", new { id = id });
            }

            var cart = _context.Carts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };

                _context.Carts.Add(cart);
            }

            var item = cart.Items
                .FirstOrDefault(i => i.PhongId == id && i.StartDate == startDate);

            if (item != null)
            {
                item.Quantity += months;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    PhongId = id,
                    Quantity = months,
                    StartDate = startDate
                });
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Cart");
        }
        // ================= VIEW CART =================
        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Phong)
                .FirstOrDefault(c => c.UserId == userId);

            return View(cart);
        }
        public IActionResult Remove(int id)
        {
            var item = _context.CartItems.FirstOrDefault(i => i.Id == id);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}