using Project.Data.Interfaces;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class RepositoryFactory
    {
        private readonly IDictionary<Type, Type> repositories = new Dictionary<Type, Type>();

        public RepositoryFactory()
        {
            repositories.Add(typeof(Customer), typeof(CustomerRepository));
            repositories.Add(typeof(Restaurant), typeof(RestaurantRepository));
            repositories.Add(typeof(Admin), typeof(AdminRepository));
            repositories.Add(typeof(Transporter), typeof(TransporterRepository));
            repositories.Add(typeof(Order), typeof(OrderRepository));
            repositories.Add(typeof(Invoice), typeof(InvoiceRepository));
            repositories.Add(typeof(Item), typeof(ItemRepository));
            repositories.Add(typeof(RestaurantAddress), typeof(RestaurantAddressRepositroy));
            repositories.Add(typeof(Review), typeof(ReviewRepository));
            repositories.Add(typeof(User), typeof(UserRepository));
        }

        public IRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            Type type = repositories[typeof(TEntity)];
            return Activator.CreateInstance(type) as IRepository<TEntity>;
        }
    }
}
