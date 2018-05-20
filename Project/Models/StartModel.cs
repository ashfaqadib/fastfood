using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class StartModel
    {
        public List<Restaurant> openRestaurants { get; set; }
        public List<Restaurant> closedRestaurants { get; set; }
        public int stars { get; set; }
        public int halfStar { get; set; }
        public StartModel()
        {
            this.openRestaurants = new List<Restaurant>();
            this.closedRestaurants = new List<Restaurant>();
        }
    }
}