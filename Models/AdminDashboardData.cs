using System;
using System.Collections.Generic;

namespace RoadReady.Models
{
    public partial class AdminDashboardData
    {
        public int DashboardId { get; set; }
        public int? TotalReservations { get; set; }
        public decimal? TotalRevenue { get; set; }
        public int? TotalUsers { get; set; }
        public int? TotalCars { get; set; }
        public int? TotalReviews { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
