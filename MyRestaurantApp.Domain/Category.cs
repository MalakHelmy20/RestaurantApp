using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
    public class Category
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;


        //audit Tables
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

          public ICollection<RestaurantCategory> RestaurantCategories { get; set; }
          = new List<RestaurantCategory>();
    }
}