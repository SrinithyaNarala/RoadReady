namespace RoadReady.Models.DTO
{
    public class CreatePaymentDTO
    {
        public int ReservationId { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
    }
}