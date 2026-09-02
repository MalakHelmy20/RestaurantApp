using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
    public class Product
    {
        
      
        public  Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

         public bool IsDeleted { get; set; } = false;

        //foreignkey & navigation property
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;
        
        //restaurantid

        //audit Tables

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }


        //to know the best sellers
  
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
