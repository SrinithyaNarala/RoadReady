using System;
using System.Collections.Generic;

namespace RoadReady.Models
{
    public partial class User
    {
        public User()
        {
            //AdminActions = new HashSet<AdminActions>();
            PasswordResetRequests = new HashSet<PasswordReset>();
            Reservations = new HashSet<Reservation>();
            Reviews = new HashSet<Review>();
            //UserAudits = new HashSet<UserAudit>();
        }

        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //public virtual ICollection<AdminActions>? AdminActions { get; set; }
        public virtual ICollection<PasswordReset>? PasswordResetRequests { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        //public virtual ICollection<UserAudit>? UserAudits { get; set; }
    }
}
