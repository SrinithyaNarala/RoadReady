namespace RoadReady.Models
{
    public class PasswordResetDTO
    {
        public int ResetId { get; set; } // Optional for create
        public int UserId { get; set; }
        public string ResetToken { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; }
    }

    // DTO for the User model (assuming User class exists)
    /* public class UserDTO
     {
         public int UserId { get; set; }
         public string Username { get; set; } // or any other fields relevant for the User
         public string Email { get; set; }
         // Include other fields from the User model as needed
     }*/
}