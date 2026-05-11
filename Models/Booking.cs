using System.ComponentModel.DataAnnotations.Schema;

namespace PHONGTROOA.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int PhongId { get; set; }   // 🔥 เปลี่ยนจาก MaPhong

        public string? UserId { get; set; }

        public DateTime BookingDate { get; set; }

        public string? Status { get; set; }

        public int Months { get; set; }

        [NotMapped]
        public decimal TongTien
        {
            get
            {
                return (Phong?.GiaThue ?? 0) * Months;
            }
        }
        public Phong? Phong { get; set; }  // ไม่ต้อง ForeignKey ก็ได้
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? RejectReason { get; set; }
        public decimal TotalPrice { get; set; }

        public DateTime? ExpiredAt { get; set; } 
        public Payment? Payment { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int People { get; set; }
        public string? Note { get; set; }
        public string? PaymentMethod { get; set; } // QR / Cash

    }
}