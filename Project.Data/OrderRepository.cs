using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class OrderRepository:Repository<Order>
    {
        public List<Order> GetOrdersByInvoiceId(int id)
        {
            return GetAll().Where(order => order.InvoiceId == id).ToList();
        }
    }
}
