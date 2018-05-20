using Project.Core.Interfaces;
using Project.Data;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public class OrderService:Service<Order>,IOrderService
    {
        OrderRepository orderRepo;
        public OrderService()
        {
            orderRepo = new OrderRepository();
        }
        public List<Order>GetOrdersByInvoiceId(int invId)
        {
            return orderRepo.GetOrdersByInvoiceId(invId);
        }
    }
}
