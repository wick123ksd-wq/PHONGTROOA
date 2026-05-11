using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Data;
using PHONGTROOA.Models;
using System.Security.Claims;

public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;

    public CheckoutController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Phong)
            .FirstOrDefault(c => c.UserId == userId);

        return View(cart);
    }
    [HttpPost]
    [Authorize]
    public IActionResult Confirm(string FullName, string Phone,
     int People, string Note, string PaymentMethod)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Phong)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null || cart.Items.Count == 0)
        {
            return Content("Cart rỗng");
        }

        var newBookings = new List<Booking>();

        foreach (var item in cart.Items)
        {
            var booking = new Booking
            {
                UserId = userId,
                PhongId = item.PhongId,
                Months = item.Quantity,
                BookingDate = DateTime.Now,
                Status = "Pending",

                StartDate = item.StartDate,
                EndDate = item.StartDate.AddMonths(item.Quantity),
                ExpiredAt = DateTime.Now.AddMinutes(30),

                FullName = FullName,
                Phone = Phone,
                People = People,
                Note = Note,
                PaymentMethod = PaymentMethod
            };

            _context.Bookings.Add(booking);
            newBookings.Add(booking);
        }

        _context.SaveChanges();

        foreach (var booking in newBookings)
        {
            var phong = _context.Phongs
                .FirstOrDefault(p => p.MaPhong == booking.PhongId);

            if (phong == null)
            {
                return Content("Không tìm thấy phòng");
            }

            var payment = new Payment
            {
                BookingId = booking.Id,
                Status = "Pending",
                Amount = phong.GiaThue * booking.Months
            };

            _context.Payments.Add(payment);
        }

        _context.SaveChanges();

        _context.CartItems.RemoveRange(cart.Items);

        _context.SaveChanges();

        return RedirectToAction("Upload", "Payment",
            new { bookingId = newBookings.First().Id });
    }
    public IActionResult Success()
    {
        return View();
    }
}