using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Entity
{
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public DateTime CheckOutTime { get; set; }

        public double Bill { get; set; }

        public double VATinPercentage { get; set; }

        public double DiscountinPercentage { get; set; }

        public double DeliveryCharge { get; set; }

        public double Discount { get; set; }

        public double VAT { get; set; }

        public int CustomerId { get; set; }

        public int RestaurantId { get; set; }

        public int TransporterId { get; set; }

        public string Status { get; set; }

        public string RestaurantName { get; set; }

        public string Recipient { get; set; }

        public string RecipientContactNo { get; set; }

        public string DeliveryAddress { get; set; }

        public string TransporterName { get; set; }

        public string TranporterContactNo { get; set; }

        public int TokenNo { get; set; }
    }
}
