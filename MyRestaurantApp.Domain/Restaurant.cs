using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
    public class Restaurant
    {
       public Guid Id { get; set; }= Guid.NewGuid();

        public string Name { get; set; }=string.Empty;
        public string  Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;




      public bool IsDeleted { get; set; } = false;
        //interval of time for open and close time
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        public bool IsOpenNow()
        {
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            return IsOpenAt(localNow.TimeOfDay);
        }

        public bool IsOpenAt(TimeSpan currentTime)
        {
            if (OpenTime == CloseTime)
            {
                return false;
            }

            if (OpenTime < CloseTime)
            {
                return currentTime >= OpenTime && currentTime < CloseTime;
            }

            // Overnight hours, e.g. 22:00 – 02:00
            return currentTime >= OpenTime || currentTime < CloseTime;
        }
          



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

        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public double AverageRating => Ratings != null && Ratings.Any() 
    ? Math.Round(Ratings.Average(r => (double)r.RatingValue), 1) 
    : 0.0;

public int TotalRatingsCount => Ratings?.Count ?? 0;


      public List<Product>Products{get;set;}=new List <Product>();
    }
}