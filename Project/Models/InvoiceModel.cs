using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class InvoiceModel
    {
        public int InvoiceNo { get; set; }
        public string OrderOwner { get; set; }
        public string OrderTime { get; set; }
        public double Bill { get; set; }
        public string Status { get; set; }
        public string RestaurantName { get; set; }
        public int TokenNo { get; set; }
    }
}