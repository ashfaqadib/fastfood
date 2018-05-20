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
    public class InvoiceService:Service<Invoice>,IInvoiceService
    {
        InvoiceRepository invRepo;
        public InvoiceService()
        {
            invRepo = new InvoiceRepository();
        }
        public List<Invoice> GetAllByRestaurantId(int id)
        {
            return invRepo.GetAllByRestaurantId(id);
        }
        public List<Invoice> GetAllByCustomerId(int id)
        {
            return invRepo.GetAllByCustomerId(id);
        }
        public List<Invoice> GetAllByTransporterId(int id)
        {
            return invRepo.GetAllByTransporterId(id);
        }
        public bool ChangeStatus(int invId, string status)
        {
            return invRepo.ChangeStatus(invId,status);
        }
    }
}
