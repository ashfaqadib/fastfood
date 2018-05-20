using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class InvoiceRepository:Repository<Invoice>
    {
        public List<Invoice> GetAllByRestaurantId(int id)
        {
            return dbContext.Set<Invoice>().Where(item => item.RestaurantId == id).ToList();
        }
        public List<Invoice> GetAllByCustomerId(int id)
        {
            return dbContext.Set<Invoice>().Where(item => item.CustomerId == id).ToList();
        }
        public List<Invoice> GetAllByTransporterId(int id)
        {
            return dbContext.Set<Invoice>().Where(item => item.TransporterId == id).ToList();
        }
        public bool ChangeStatus(int invId, string status)
        {
            Invoice inv = Get(invId);
            inv.Status = status;
            return Update(inv,invId);

        }

        public Invoice PrepareInvoice(Customer cust,Restaurant rest, double grandTotal)
        {

            double bill = grandTotal, discount = 0, vat = 0;

            discount = ((rest.DiscountinPercentage * bill) / 100);
            bill -= discount;
            vat = ((rest.VATinPercentage * bill) / 100);
            bill += vat;
            bill += rest.DeliveryCharge;

            bill = Math.Ceiling(bill);

            Invoice inv = new Invoice();
            inv.Id = dbContext.Invoices.Count() + 1;
            inv.CustomerId = cust.Id;
            inv.Bill = bill;
            inv.VATinPercentage = rest.VATinPercentage;
            inv.DiscountinPercentage = rest.DiscountinPercentage;
            inv.DeliveryCharge = rest.DeliveryCharge;
            inv.Discount = discount;
            inv.VAT = vat;
            inv.Status = "Pending";
            inv.Recipient = cust.Name;
            inv.RecipientContactNo = cust.ContactNumber;
            inv.DeliveryAddress = cust.DeliveryAddress;
            inv.RestaurantName = rest.Name;
            inv.TokenNo = new Random().Next(10000, 30000);

            inv.CheckOutTime = DateTime.Now;
            inv.RestaurantId = rest.Id;

            return inv;

        }
    }
}
