using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class TransporterRepository:Repository<Transporter>
    {
        InvoiceRepository invRepo;
        TransporterTaskRepository trnTaskRepo;

        public TransporterRepository()
        {
            this.invRepo = new InvoiceRepository();
            this.trnTaskRepo = new TransporterTaskRepository();
        }

        public bool UpdateInvoice(int transporterId,int invId,string status)
        {
            Invoice inv = invRepo.Get(invId);
            if (transporterId != 0)
            {
                Transporter transporter = Get(transporterId);
                inv.Status = status;
                inv.TranporterContactNo = transporter.ContactNumber;
                inv.TransporterId = transporter.Id;
                inv.TransporterName = transporter.Name;
            }
            else
            {
                inv.TranporterContactNo = null;
                inv.TransporterId = 0;
                inv.TransporterName = null;
                inv.Status = status;
            }
            return invRepo.Update(inv,invId);
        }
        public List<Invoice> GetDeliveryInvoices(int id)
        {
            return invRepo.GetAllByTransporterId(id);
        }
        public bool AddTask(int invId,int transporterId)
        {
            TransporterTask alreadyExisitingTask = trnTaskRepo.GetTasksByTransporterId(transporterId).Where(taks => taks.InvoiceId == invId).SingleOrDefault();
            if (alreadyExisitingTask != null)
            {
                alreadyExisitingTask.Status = "On The Way";
                alreadyExisitingTask.StartTime = DateTime.Now;
                trnTaskRepo.Update( alreadyExisitingTask,alreadyExisitingTask.Id);
            }
            else
            {
                TransporterTask task = new TransporterTask();
                task.InvoiceId = invId;
                task.StartTime = DateTime.Now;
                task.Status = "On The Way";
                task.TransporterId = transporterId;
                trnTaskRepo.Insert(task);
            }
            return true;
        }

        public bool CompleteTask(int trnId, int invId)
        {
            TransporterTask task = trnTaskRepo.GetTasksByTransporterId(trnId).Where(t => t.InvoiceId == invId).SingleOrDefault();
            task.Status = "Completed";
            task.EndTime = DateTime.Now;
            return trnTaskRepo.Update(task, task.Id);
        }

        public List<TransporterTask> GetTasks(int trnId)
        {
            return trnTaskRepo.GetAll().Where(task => task.TransporterId == trnId).ToList();
        }

        public bool CancelTask(int trnId,int invId)
        {
            TransporterTask task = trnTaskRepo.GetTasksByTransporterId(trnId).Where(t => t.InvoiceId==invId).SingleOrDefault();
            task.Status = "Cancelled";
            task.EndTime = DateTime.Now;
            return trnTaskRepo.Update(task, task.Id);
        }
        public Invoice GetInvoiceDetails(int id)
        {
            return invRepo.Get(id);
        }
        public bool Delete(Transporter transporterodelete)
        {
            dbContext.Transporters.Remove(transporterodelete);

            User user = dbContext.Users.Where(i => i.Id == transporterodelete.Id).Select(i => i).SingleOrDefault();
            dbContext.Users.Remove(user);

            return dbContext.SaveChanges() > 0;
        }
    }
}
