using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface IItemService:IService<Item>
    {
        List<Item> GetAllByRestaurantId(int id);
        Item GetByOrder(Order order);
    }
}
