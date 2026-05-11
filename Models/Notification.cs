using Microsoft.AspNetCore.Mvc;

namespace PHONGTROOA.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string UserId { get; set; } // คนที่ได้รับ

        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? BookingId { get; set; }
    }
}
