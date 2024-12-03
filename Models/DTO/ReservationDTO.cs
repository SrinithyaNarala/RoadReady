namespace RoadReady.Models.DTO
{
    public class ReservationDTO
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CarExtraDTO> Extras { get; set; }
        public List<int> CarExtraIds { get; set; } 
        public string Status { get; set; }
    }
}