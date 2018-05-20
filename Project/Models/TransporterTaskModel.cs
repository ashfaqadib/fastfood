using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class TransporterTaskModel
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContactNo { get; set; }
        public int InvoiceNo { get; set; }
        public string DeliveryAddress { get; set; }
        public string StartingTime { get; set; }
        public string EndTime { get; set; }
        public string Status { get; set; }
    }
}