using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface ITransporterService:IService<Transporter>
    {
        bool DeliverOrder(int transporterId, int invId, string status);
        bool AddTask(int transporterId, int invId);
        List<TransporterTask> GetTasks(int transporterId);
        List<Invoice> GetDeliveryInvoices(int transporterId);
        bool CancelTask(int transporterId, int invId, string status);
        bool ConfirmDelivery(int transporterId, int invId, int token);
        bool Delete(Transporter transporterodelete);
    }
}
