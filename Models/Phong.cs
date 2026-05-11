using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace PHONGTROOA.Models  
{
    public class Phong
    {
        [Key]
        public int MaPhong { get; set; }

        [Required]
        public string TenPhong { get; set; } = string.Empty;

        public decimal GiaThue { get; set; }

        public double DienTich { get; set; }

        public string DiaChi { get; set; } = string.Empty;

        public string? HinhAnh { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public bool PhongHot { get; set; }

        public int MaLoai { get; set; }

        public DateTime NgayDang { get; set; }
    }
}