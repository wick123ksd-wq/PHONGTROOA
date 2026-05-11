using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Data;
using PHONGTROOA.Models;

namespace PHONGTROOA.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PaymentController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ================= LIST =================
        public IActionResult Index()
        {
            var payments = _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Phong)
                .ToList();

            return View(payments);
        }

        // ================= UPLOAD =================
        public IActionResult Upload(int bookingId)
        {
            var booking = _context.Bookings
      .Include(b => b.Phong)   // 🔥 ต้องมี
      .Include(b => b.Payment)
      .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
                return Content("Không tìm thấy booking");

            // ส่งจำนวนเงินไป View
            ViewBag.Amount = booking.TongTien;
            ViewBag.BookingId = booking.Id;

            return View(booking.Payment);
        }

        [HttpPost]
        public IActionResult Upload(Payment model)
        {
            var payment = _context.Payments
                .FirstOrDefault(p => p.BookingId == model.BookingId);

            if (payment == null)
                return Content("Không tìm thấy payment");

            // ❌ กัน upload ซ้ำ
            if (!string.IsNullOrEmpty(payment.SlipImage))
            {
                return Content("Bạn đã upload rồi!");
            }

            if (model.SlipFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString()
                    + Path.GetExtension(model.SlipFile.FileName);

                string path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    model.SlipFile.CopyTo(stream);
                }

                payment.SlipImage = "/uploads/" + fileName;
                payment.Status = "Uploaded"; // 🔥 รอ admin
            }

            _context.SaveChanges();

            return RedirectToAction("Success");
        }

        // ================= VIEW SLIP =================
        public IActionResult View(int id)
        {
            var payment = _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Phong)
                .FirstOrDefault(p => p.BookingId == id);

            return View(payment);
        }

        // ================= APPROVE =================
        public IActionResult Approve(int id)
        {
            var payment = _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefault(p => p.Id == id);

            if (payment == null) return NotFound();

            payment.Status = "Approved";
            payment.Booking.Status = "Confirmed";

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= REJECT =================
        public IActionResult Reject(int id)
        {
            var payment = _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefault(p => p.Id == id);

            if (payment == null) return NotFound();

            payment.Status = "Rejected";
            payment.Booking.Status = "Cancelled";

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Success()
        {
            return View("~/Views/Checkout/Success.cshtml");
        }
    }
}