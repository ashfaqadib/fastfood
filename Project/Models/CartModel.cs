using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class CartModel
    {
        public string RestaurantName { get; set; }
        public List<Item> Item { get; set; }
        public List<int> Quantity { get; set; }
        public List<double> Price { get; set; }
        public string Total { get; set; }
        public string Bill { get; set; }
        public string VATinPercentage { get; set; }
        public string DiscountinPercentage { get; set; }
        public string DeliveryCharge { get; set; }
        public string Discount { get; set; }
        public string VAT { get; set; }
        public string InvoiceNo { get; set; }
        public string RestaurantAddress { get; set; }
        public string RecipientContactNo { get; set; }
        public string Recipient { get; set; }
        public string DeliveryAddress { get; set; }
       

        public CartModel()
        {
            this.Item = new List<Item>();
            this.Quantity = new List<int>();
            this.Price = new List<double>();
        }
    }
}