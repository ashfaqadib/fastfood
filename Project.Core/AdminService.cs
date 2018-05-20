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
    public class AdminService:Service<Admin>,IAdminService
    {
        AdminRepository adminRepo;
        public AdminService()
        {
            adminRepo = new AdminRepository();
        }
        public bool Delete(Admin admintodelete)
        {
            return adminRepo.Delete(admintodelete);
        }
        public IEnumerable<TEntity> GetByName<TEntity>(string name) where TEntity : class
        {
            return adminRepo.GetByName<TEntity>(name);
        }
        public IEnumerable<TEntity> GetByEmail<TEntity>(string email) where TEntity : class
        {
            return adminRepo.GetByEmail<TEntity>(email);
        }
        public IEnumerable<TEntity> GetByStatus<TEntity>(string status) where TEntity : class
        {
            return adminRepo.GetByEmail<TEntity>(status);
        }
        public Customer DetailsOfCustomer(string email)
        {
            return adminRepo.DetailsOfCustomer(email);
        }
        public int GetIdFromEmail<TEntity>(string email) where TEntity : class
        {
            return adminRepo.GetIdFromEmail<TEntity>(email);
        }
    }
}
