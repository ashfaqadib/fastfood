using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class ItemRepository:Repository<Item>
    {
        public List<Item> GetAllByRestaurantId(int id)
        {
            return dbContext.Set<Item>().Where(item => item.RestaurantId == id).ToList();
        }
        public Item GetByOrder(Order order)
        {
            return dbContext.Items.Find(order.ItemId);
        }
    }
}
