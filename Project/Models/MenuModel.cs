using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class MenuModel
    {
        public List<List<Item>> Items { get; set; }
        public List<Order> Orders { get; set; }
        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public string Address { get; set; }
        public string ImageLocation { get; set; }
    }
}