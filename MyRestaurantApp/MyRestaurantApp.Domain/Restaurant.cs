using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
    public class Restaurant
    {
       public Guid Id { get; set; }= Guid.NewGuid();

        public string Name { get; set; }=string.Empty;
        public string   Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;





        //interval of time for open and close time
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
          



        //foreignkey & navigation property
        public Guid  OwnerId { get; set; }
        public User Owner { get; set; } = null!;




        //audit Tables
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        
        //lists

      public ICollection<RestaurantCategory> RestaurantCategories { get; set; }
        = new List<RestaurantCategory>();
        public List<Order> Orders { get; set; } = new List<Order>();

     //to know all the ratings

        public List<Ratings> Ratings { get; set; } = new List<Ratings>();



    }
}