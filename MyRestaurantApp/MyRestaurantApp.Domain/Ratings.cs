using System;
using System.Collections.Generic;
using System.Text;
namespace MyRestaurantApp.Domain{
public class Ratings
{
    public Guid Id { get; set; }

    //foreign key & navigation property
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    //fk & navigation property
    public Guid RestaurantId { get; set; }
     public Restaurant Restaurant { get; set; } = null!;
 
       //with constraints, rating should be between 1 and 5
       public int Rating { get; set; } 
   
     //audit Tables

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    

}
}