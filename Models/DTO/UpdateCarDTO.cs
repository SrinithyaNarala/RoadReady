namespace RoadReady.Models.DTO
{
    public class UpdateCarDTO
    {
        public int CarId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public string CarType { get; set; }  
        public string Location { get; set; }  
        public bool Availability { get; set; } 
        public string ImageUrl { get; set; }
    }
}