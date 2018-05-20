using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface IRestaurantService:IService<Restaurant>
    {
        RestaurantAddress GetRestaurantAddress(int restId);
        Restaurant GetByInvoice(Invoice inv);
        Restaurant GetByOrder(Order order);
        bool UpdateAddress(int id, RestaurantAddress address);
        bool Delete(Restaurant restodelete);
    }
}
