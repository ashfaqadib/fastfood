using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class AdminRepository : Repository<Admin>
    {
        public bool Delete(Admin admintodelete)
        {
            dbContext.Admins.Remove(admintodelete);
            return dbContext.SaveChanges() > 0;
        }

        public IEnumerable<TEntity> GetByName<TEntity>(string name) where TEntity : class
        {
            List<TEntity> list = new List<TEntity>();
            if (typeof(TEntity) == typeof(Customer))
            {
                list = dbContext.Customers.Where(a => a.Name == name).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Restaurant))
            {
                list = dbContext.Restaurants.Where(a => a.Name == name).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Admin))
            {
                list = dbContext.Admins.Where(a => a.Name == name).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Transporter))
            {
                list = dbContext.Transporters.Where(a => a.Name == name).Select(a => a).ToList() as List<TEntity>;
            }
            return list as IEnumerable<TEntity>;
        }

        public IEnumerable<TEntity> GetByEmail<TEntity>(string email) where TEntity : class
        {
            List<TEntity> list = new List<TEntity>();
            if (typeof(TEntity) == typeof(Customer))
            {
                list = dbContext.Customers.Where(a => a.Email == email).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Restaurant))
            {
                list = dbContext.Restaurants.Where(a => a.Email == email).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Admin))
            {
                list = dbContext.Admins.Where(a => a.Email == email).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Transporter))
            {
                list = dbContext.Transporters.Where(a => a.Email == email).Select(a => a).ToList() as List<TEntity>;
            }
            return list;
        }

        public IEnumerable<TEntity> GetByStatus<TEntity>(string status) where TEntity : class
        {
            List<TEntity> list = new List<TEntity>();
            if (typeof(TEntity) == typeof(Customer))
            {
                list = dbContext.Customers.Where(a => a.Status == status).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Restaurant))
            {
                list = dbContext.Restaurants.Where(a => a.Status == status).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Admin))
            {
                list = dbContext.Admins.Where(a => a.Status == status).Select(a => a).ToList() as List<TEntity>;
            }
            else if (typeof(TEntity) == typeof(Transporter))
            {
                list = dbContext.Transporters.Where(a => a.Status == status).Select(a => a).ToList() as List<TEntity>;
            }
            return list;
        }


        public Customer DetailsOfCustomer(string email)
        {
            Customer cust = dbContext.Customers.Where(a => a.Email == email).Select(a => a).SingleOrDefault();
            return cust;
        }

        public int GetIdFromEmail<TEntity>(string email) where TEntity : class
        {
            int id = 0;
            if (typeof(TEntity) == typeof(Customer))
            {
                id = dbContext.Customers.Where(a => a.Email == email).Select(a => a.Id).SingleOrDefault();
            }
            else if (typeof(TEntity) == typeof(Restaurant))
            {
                id = dbContext.Restaurants.Where(a => a.Email == email).Select(a => a.Id).SingleOrDefault();
            }
            else if (typeof(TEntity) == typeof(Admin))
            {
                id = dbContext.Admins.Where(a => a.Email == email).Select(a => a.Id).SingleOrDefault();
            }
            else if (typeof(TEntity) == typeof(Transporter))
            {
                id = dbContext.Transporters.Where(a => a.Email == email).Select(a => a.Id).SingleOrDefault();
            }
            return id;
        }

    }
}
