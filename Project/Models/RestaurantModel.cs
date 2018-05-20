using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class RestaurantModel
    {
        [Required]
        public string Name { set; get; }
        [Required]
        public string Status { set; get; }
        [Required]
        public string Email { set; get; }
    }
}