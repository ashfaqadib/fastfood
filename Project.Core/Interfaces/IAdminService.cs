using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface IAdminService:IService<Admin>
    {
        IEnumerable<TEntity> GetByName<TEntity>(string name) where TEntity : class;
        bool Delete(Admin admintodelete);
        IEnumerable<TEntity> GetByEmail<TEntity>(string email) where TEntity : class;
        IEnumerable<TEntity> GetByStatus<TEntity>(string status) where TEntity : class;
        Customer DetailsOfCustomer(string email);
        int GetIdFromEmail<TEntity>(string email) where TEntity : class;
    }
}
