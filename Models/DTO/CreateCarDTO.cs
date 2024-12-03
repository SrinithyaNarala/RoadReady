
namespace RoadReady.Models.DTO
{
    public class CreateCarDTO
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public string CarType { get; set; }  // Type of car (e.g., Sedan, SUV, etc.)
        public string Location { get; set; }  // Location of the car
        public bool Availability { get; set; }  // Whether the car is available for rent
        public string ImageUrl { get; set; }
    }
}
