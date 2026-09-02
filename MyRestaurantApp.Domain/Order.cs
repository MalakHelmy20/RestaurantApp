using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
    public  enum statusTypes
    {
        //confirm-preparing -OnDelivery -Delivered- Cancelled
        confirm = 1,
        preparing = 2,
        onDelivery = 3,
        delivered = 4,
        cancelled = 5
    }
public class Order
    {
        public Guid Id { get; set; }= Guid.NewGuid();

        public statusTypes status { get; set; }


        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }

         public bool IsDeleted { get; set; } = false;


        //foreignkey & navigation property
        
        public Guid CustomerId { get; set; }
        public User Customer { get; set; } = null!;
        

        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;


        //audit Tables
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

}
}