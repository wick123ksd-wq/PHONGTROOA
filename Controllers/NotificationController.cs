using Microsoft.AspNetCore.Mvc;
using PHONGTROOA.Data;
using System.Security.Claims;

namespace PHONGTROOA.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notifications = _context.Notifications
                .Where(n => n.UserId.ToString() == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            foreach (var item in notifications)
            {
                item.IsRead = true;
            }

            _context.SaveChanges();

            return View(notifications);
        }
    }
}
