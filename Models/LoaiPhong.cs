using System.ComponentModel.DataAnnotations;

namespace PHONGTROOA.Models
{
    public class LoaiPhong
    {
        [Key]
        public int MaLoai { get; set; }

        [Required]
        public string TenLoai { get; set; } = string.Empty;
    }
}