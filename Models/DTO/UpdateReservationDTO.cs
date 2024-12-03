namespace RoadReady.Models.DTO
{
    public class UpdateReservationDTO
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<int> CarExtraIds { get; set; } 
        public string Status { get; set; }
    }
}