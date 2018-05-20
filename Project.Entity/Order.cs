using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int InvoiceId { get; set; }
    }
}
