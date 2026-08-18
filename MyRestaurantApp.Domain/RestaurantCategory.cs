using System;
using System.Collections.Generic;
using System.Text;

namespace MyRestaurantApp.Domain
{
    public class RestaurantCategory
    {
        //restaurant
        public Guid RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;


        //categories
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}