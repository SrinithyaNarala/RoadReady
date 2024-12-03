using System;
using System.Collections.Generic;

namespace RoadReady.Models
{
    public partial class Car
    {
        public Car()
        {
            Reservations = new HashSet<Reservation>();
            Reviews = new HashSet<Review>();
        }

        public int CarId { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public decimal? PricePerDay { get; set; }
        public string? CarType { get; set; }
        public string? Location { get; set; }
        public bool? Availability { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}