using Microsoft.AspNetCore.Mvc;
using PHONGTROOA.Data;
using PHONGTROOA.Models;

public class ContactController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContactController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(Contact model)
    {
        if (ModelState.IsValid)
        {
            _context.Contacts.Add(model);
            _context.SaveChanges();
            ViewBag.Message = "Gửi liên hệ thành công!";
        }

        return View();
    }
}
