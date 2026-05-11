using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace PHONGTROOA.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required] // 🔥 ป้องกัน null
        public int BookingId { get; set; }

        [ForeignKey("BookingId")] // 🔥 สำคัญมาก
        public Booking? Booking { get; set; }

        [Required] // 🔥 ป้องกัน error DB
        public string Status { get; set; } = "Pending";
        // Pending | Uploaded | Approved | Rejected

        public string? SlipImage { get; set; }

        [NotMapped]
        public IFormFile? SlipFile { get; set; }
        public decimal Amount { get; set; }
    }
}