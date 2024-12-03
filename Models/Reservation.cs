using RoadReady.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoadReady.Models
{
    public partial class Reservation
    {
        public Reservation()
        {
            Payments = new HashSet<Payment>();
            Extras = new HashSet<CarExtra>();
            CarExtraIds = new List<int>();
        }

        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        [DateRange("PickupDate", ErrorMessage = "Drop-off date must be after the pickup date.")]
        public DateTime DropoffDate { get; set; }
        public decimal TotalPrice { get; set; }

        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Car? Car { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<CarExtra> Extras { get; set; }
        public List<int> CarExtraIds { get; set; }
    }
}
