using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface IInvoiceService:IService<Invoice>
    {
        List<Invoice> GetAllByCustomerId(int id);
        List<Invoice> GetAllByRestaurantId(int id);
        List<Invoice> GetAllByTransporterId(int id);
        bool ChangeStatus(int invId, string status);
    }
}
