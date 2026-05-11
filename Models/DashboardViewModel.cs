using Microsoft.AspNetCore.Mvc;

namespace PHONGTROOA.Models
{
    public class DashboardViewModel
    {
        public int TotalRooms { get; set; }

        public int TotalBookings { get; set; }

        public int PendingBookings { get; set; }

        public int ApprovedBookings { get; set; }
    }
}

