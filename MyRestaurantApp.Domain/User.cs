namespace MyRestaurantApp.Domain
{
 
    public enum UserRole
    {
        SystemAdmin = 1,
        Customer = 2,
        RestaurantOwner = 3
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Customer;


        //audit Tables

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        //list
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        
        public List<Order> Orders { get; set; } = new List<Order>();


    }
}
