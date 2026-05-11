using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Data;
using PHONGTROOA.Models;
using System.Linq;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Bookings");
    }

    public IActionResult Dashboard()
    {
        ViewBag.RoomCount = _context.Phongs.Count();

        ViewBag.BookingCount = _context.Bookings.Count();

        ViewBag.PendingCount = _context.Bookings
            .Count(b => b.Status == "Pending");

        ViewBag.ConfirmedCount = _context.Bookings
            .Count(b => b.Status == "Confirmed");

        ViewBag.Revenue = _context.Payments
            .Where(p => p.Status == "Approved")
            .Sum(p => (decimal?)p.Amount) ?? 0;

        return View();
    }

    public IActionResult Rooms()
    {
        var rooms = _context.Phongs.ToList();
        return View(rooms);
    }

    // 🔥 FIX ตรงนี้
    public IActionResult Bookings()
    {
        var bookings = _context.Bookings
            .Include(b => b.Phong)   // 🔥 สำคัญ
            .Include(b => b.Payment) // 🔥 สำคัญ
            .OrderByDescending(b => b.Id)
            .ToList();

        return View(bookings);
    }

    // ✅ Approve (เวอร์ชันโปร)
    public IActionResult Approve(int id)
    {
        var booking = _context.Bookings
            .Include(b => b.Payment)
            .FirstOrDefault(b => b.Id == id);

        if (booking == null)
            return Content("Not found");

        booking.Status = "Confirmed";

        if (booking.Payment != null)
        {
            booking.Payment.Status = "Approved";
        }

        // 🔥 Notification
        var noti = new Notification
        {
            UserId = booking.UserId,
            BookingId = booking.Id,

            Message = "✅ Đơn đặt phòng của bạn đã được duyệt!"
        };

        _context.Notifications.Add(noti);

        _context.SaveChanges();

        return RedirectToAction("Bookings");
    }

    // ❌ Reject
    public IActionResult Reject(int id)
    {
        var booking = _context.Bookings
            .Include(b => b.Payment)
            .FirstOrDefault(b => b.Id == id);

        if (booking == null)
            return Content("Không tìm thấy");

        booking.Status = "Rejected";

        if (booking.Payment != null)
        {
            booking.Payment.Status = "Rejected";
        }

        // 🔥 Notification
        var noti = new Notification
        {
            UserId = booking.UserId,
            BookingId = booking.Id,

            Message = "❌ Đơn đặt phòng của bạn đã bị từ chối!"
        };

        _context.Notifications.Add(noti);

        _context.SaveChanges();

        return RedirectToAction("Bookings");
    }
}