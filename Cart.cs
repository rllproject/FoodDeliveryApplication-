using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodOrderSystem.Models
{
    public class Cart
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string productPic { get; set; }
        public decimal price { get; set; }
        public int qty { get; set; }
        public decimal bill { get; set; }
    }
}