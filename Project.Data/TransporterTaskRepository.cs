using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    class TransporterTaskRepository:Repository<TransporterTask>
    {
        public List<TransporterTask> GetTasksByTransporterId(int trnId)
        {
            return GetAll().Where(task => task.TransporterId == trnId).ToList();
        }
    }
}
