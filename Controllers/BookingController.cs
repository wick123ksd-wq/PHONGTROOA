using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Data;
using PHONGTROOA.Models;
using System.Security.Claims;

namespace PHONGTROOA.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= ADMIN =================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Phong)
                .Include(b => b.Payment)
                .OrderByDescending(b => b.Id)
                .ToList();

            return View(bookings);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null) return NotFound();

            booking.Status = "Confirmed";
            booking.Payment.Status = "Approved";

            // 🔥 CREATE NOTIFICATION
            var notification = new Notification
            {
                UserId = booking.UserId,
                BookingId = booking.Id,
                Message = "✅ Đơn đặt phòng của bạn đã được duyệt!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= REJECT =================
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null) return NotFound();

            booking.Status = "Cancelled";
            booking.Payment.Status = "Rejected";

            // 🔥 CREATE NOTIFICATION
            var notification = new Notification
            {
                UserId = booking.UserId,
                BookingId = booking.Id,
                Message = "❌ Đơn đặt phòng của bạn đã bị từ chối!",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // ================= CREATE =================
        [HttpGet]
        public IActionResult Create(int roomId)
        {
            ViewBag.RoomId = roomId;
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            var phong = _context.Phongs.Find(booking.PhongId);

            booking.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            booking.BookingDate = DateTime.Now;
            booking.Status = "Pending";

            booking.TotalPrice = booking.Months * (phong?.GiaThue ?? 0);

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // 🔥 tạo payment
            var payment = new Payment
            {
                BookingId = booking.Id,
                Status = "Pending"
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Id == id);

            if (booking != null)
            {
                booking.Status = "Cancelled";

                if (booking.Payment != null)
                {
                    booking.Payment.Status = "Cancelled";
                }

                _context.SaveChanges();
            }

            return RedirectToAction("MyBookings");
        }
        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return Content("Không tìm thấy");

            // 🔥 เช็คหมดอายุ
            var isExpired = booking.ExpiredAt != null &&
                            booking.ExpiredAt < DateTime.Now;

            // 🔥 อนุญาตลบเฉพาะเคสนี้
            var canDelete =
                booking.Status == "Cancelled" ||
                booking.Status == "Expired" ||
                booking.Payment?.Status == "Rejected" ||
                isExpired;

            if (!canDelete)
            {
                return Content("❌ Không thể xóa booking này");
            }

            // 🔥 ลบ Payment ก่อน (กัน FK error)
            if (booking.Payment != null)
            {
                _context.Payments.Remove(booking.Payment);
            }

            // 🔥 ลบ Booking
            _context.Bookings.Remove(booking);

            _context.SaveChanges();

            return RedirectToAction("MyBookings");
        }
        [Authorize]
        public IActionResult MyBookings()
        {
            CheckExpiredBookings(); // 🔥 ต้องมี

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var bookings = _context.Bookings
                .Include(b => b.Phong)   // 🔥 เพิ่มบรรทัดนี้
                .Include(b => b.Payment) // 🔥 และอันนี้
                .Where(b => b.UserId == userId)
                .ToList();

            return View(bookings);
        }
        public IActionResult Detail(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Phong)
                .Include(b => b.Payment)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        private void CheckExpiredBookings()
        {
            var expired = _context.Bookings
     .Where(b =>
         b.Status == "Pending" // 🔥 สำคัญ
         && b.ExpiredAt != null
         && b.ExpiredAt < DateTime.Now)
     .ToList();

            foreach (var b in expired)
            {
                b.Status = "Expired";

                var payment = _context.Payments
                    .FirstOrDefault(p => p.BookingId == b.Id);

                if (payment != null)
                {
                    payment.Status = "Expired";
                }
            }

            _context.SaveChanges();
        }
    }
}