using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class AddressModel
    {
        [Required]
        public string Flat { get; set; }

        [Required]
        public string House { get; set; }

        [Required]
        public string Road { get; set; }

        [Required]
        public string Area { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        public string Contact { get; set; }
    }
}