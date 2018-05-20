using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class CustomerRepository:Repository<Customer>
    {
            public bool Delete(Customer custodelete)
            {
                dbContext.Customers.Remove(custodelete);

                User user = dbContext.Users.Where(i => i.Id == custodelete.Id).Select(i => i).SingleOrDefault();
                dbContext.Users.Remove(user);

                return dbContext.SaveChanges() > 0;
            }
    }
}
