using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PHONGTROOA.Data;
using PHONGTROOA.Models;
using Microsoft.AspNetCore.Authorization;

namespace PHONGTROOA.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================= Index =================
        public IActionResult Index(string keyword, string sortOrder, string dientich)
        {
            var dsPhong = _context.Phongs.AsQueryable();

            // SEARCH
            if (!string.IsNullOrEmpty(keyword))
            {
                dsPhong = dsPhong.Where(p =>
                    p.TenPhong.Contains(keyword) ||
                    p.DiaChi.Contains(keyword));
            }

            // FILTER DIỆN TÍCH
            if (dientich == "1")
                dsPhong = dsPhong.Where(p => p.DienTich < 20);

            if (dientich == "2")
                dsPhong = dsPhong.Where(p => p.DienTich >= 20 && p.DienTich <= 40);

            if (dientich == "3")
                dsPhong = dsPhong.Where(p => p.DienTich > 40 && p.DienTich <= 60);

            if (dientich == "4")
                dsPhong = dsPhong.Where(p => p.DienTich > 60);

            // SORT
            switch (sortOrder)
            {
                case "price_asc":
                    dsPhong = dsPhong.OrderBy(p => p.GiaThue);
                    break;

                case "price_desc":
                    dsPhong = dsPhong.OrderByDescending(p => p.GiaThue);
                    break;

                default:
                    dsPhong = dsPhong.OrderByDescending(p => p.NgayDang);
                    break;
            }

            return View(dsPhong.ToList());
        }

        // ================= ChiTiet =================
        public async Task<IActionResult> ChiTiet(int id)
        {
            var phong = await _context.Phongs
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phong == null)
                return NotFound();

            return View(phong);
        }









        // ================= Create GET =================
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // ================= Create POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Phong phong)
        {
            if (ModelState.IsValid)
            {
                // ถ้ามีการอัปโหลดรูป
                if (phong.ImageFile != null)
                {
                    // สร้างชื่อไฟล์ใหม่กันชื่อซ้ำ
                    var fileName = Guid.NewGuid().ToString()
                                   + Path.GetExtension(phong.ImageFile.FileName);

                    // กำหนด path โฟลเดอร์
                    var uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images"
                    );

                    // ถ้าไม่มีโฟลเดอร์ ให้สร้าง
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // path เต็ม
                    var filePath = Path.Combine(uploadPath, fileName);

                    // save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await phong.ImageFile.CopyToAsync(stream);
                    }

                    // บันทึก path ลง database
                    phong.HinhAnh = "/images/" + fileName;
                }

                phong.NgayDang = DateTime.Now;

                _context.Phongs.Add(phong);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(phong);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null)
                return NotFound();

            return View(phong);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Phong phong)
        {
            if (id != phong.MaPhong)
                return NotFound();

            if (ModelState.IsValid)
            {
                var phongDb = await _context.Phongs.FindAsync(id);
                if (phongDb == null) return NotFound();

                // อัปเดตข้อมูลปกติ
                phongDb.TenPhong = phong.TenPhong;
                phongDb.GiaThue = phong.GiaThue;
                phongDb.DienTich = phong.DienTich;
                phongDb.DiaChi = phong.DiaChi;

                // ถ้ามีรูปใหม่
                if (phong.ImageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString()
                                   + Path.GetExtension(phong.ImageFile.FileName);

                    var uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images"
                    );

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await phong.ImageFile.CopyToAsync(stream);
                    }

                    phongDb.HinhAnh = "/images/" + fileName;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(phong);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var phong = _context.Phongs.Find(id);
            if (phong == null) return NotFound();
            return View(phong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var phong = _context.Phongs.Find(id);
            if (phong == null) return NotFound();

            _context.Phongs.Remove(phong);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}