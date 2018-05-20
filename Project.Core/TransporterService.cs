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
    public class TransporterService : Service<Transporter>, ITransporterService
    {
        TransporterRepository transporterRepo;

        public TransporterService()
        {
            this.transporterRepo = new TransporterRepository();
        }

        public bool DeliverOrder(int transporterId, int invId,string status)
        {
            return transporterRepo.UpdateInvoice(transporterId, invId, status) && transporterRepo.AddTask(invId, transporterId);
        }

        public bool AddTask(int transporterId, int invId)
        {
            return transporterRepo.AddTask(transporterId, invId);
        }

        public List<TransporterTask> GetTasks(int transporterId)
        {
            return transporterRepo.GetTasks(transporterId);
        }


        public List<Invoice> GetDeliveryInvoices(int transporterId)
        {
            return transporterRepo.GetDeliveryInvoices(transporterId);
        }

        public Invoice GetInvoiceDetails(int invId)
        {
            return transporterRepo.GetInvoiceDetails(invId);
        }

        public bool CancelTask(int transporterId,int invId, string status)
        {
            return transporterRepo.UpdateInvoice(0, invId, status) && transporterRepo.CancelTask(transporterId,invId);
        }

        public bool ConfirmDelivery(int transporterId, int invId, int token)
        {
            Invoice invoice = transporterRepo.GetDeliveryInvoices(transporterId).Where(inv => inv.Id == invId).SingleOrDefault();
            if (invoice.TokenNo == token)
            {
                return transporterRepo.UpdateInvoice(transporterId, invId, "Delivered") && transporterRepo.CompleteTask(transporterId,invId);
            }
            else return false;
        }
        public bool Delete(Transporter transporterodelete)
        {
            return transporterRepo.Delete(transporterodelete);
        }
    }
}
