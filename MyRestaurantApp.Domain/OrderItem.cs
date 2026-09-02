using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
   public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        //foreignkey & navigation property
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public decimal TotalPrice => Quantity * UnitPrice;


        public Guid ProductId { get; set; }
        public Product Product { get; set; }= null!;

        //audit Tables

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
