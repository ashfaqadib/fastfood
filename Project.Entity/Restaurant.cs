using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Entity
{
    public class Restaurant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime LastOnline { get; set; }

        public string Status { get; set; }

        public int AddressId { get; set; }

        public string ImageLocation { get; set; }

        public string OpenHours { get; set; }

        public double MinimumOrder { get; set; }

        public double Rating { get; set; }

        public double DeliveryCharge { get; set; }

        public double DiscountinPercentage { get; set; }

        public double VATinPercentage { get; set; }
    }
}
