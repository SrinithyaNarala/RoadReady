using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RoadReady.Models
{
    public partial class CarExtra
    {
        public CarExtra()
        {
            Reservation = new HashSet<Reservation>();
        }

        public int ExtraId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }

        [JsonIgnore]
        public virtual ICollection<Reservation>? Reservation { get; set; }
    }
}
