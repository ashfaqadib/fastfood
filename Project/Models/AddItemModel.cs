using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class AddItemModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Proportion { get; set; }
        [Required]
        [Range(1.0, Single.MaxValue, ErrorMessage = "Please input a valid price.")]
        public int Price { get; set; }

        public int RestaurantId { get; set; }
    }
}