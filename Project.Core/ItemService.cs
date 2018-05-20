using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Core.Interfaces;
using Project.Entity;
using Project.Data;

namespace Project.Core
{
    public class ItemService:Service<Item>,IItemService
    {
        ItemRepository itemRepo;
        public ItemService()
        {
            itemRepo = new ItemRepository();
        }
        public List<Item> GetAllByRestaurantId(int id)
        {
            return itemRepo.GetAllByRestaurantId(id);
        }
        public Item GetByOrder(Order order)
        {
            return itemRepo.GetByOrder(order);
        }
    }
}
