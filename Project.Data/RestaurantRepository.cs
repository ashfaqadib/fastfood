using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
        public class RestaurantRepository : Repository<Restaurant>
        {
            public bool Delete(Restaurant restodelete)
            {
                dbContext.Restaurants.Remove(restodelete);

                User user = dbContext.Users.Where(i => i.Id == restodelete.Id).Select(i => i).SingleOrDefault();
                dbContext.Users.Remove(user);

                return dbContext.SaveChanges() > 0;
            }
            public Restaurant GetByItem(Item item)
            {
                return dbContext.Restaurants.Find(item.RestaurantId);
            }
            public Restaurant GetByInvoice(Invoice inv)
            {
                return dbContext.Restaurants.Find(inv.RestaurantId);
            }
            public Restaurant GetByOrder(Order order)
            {
                List<Item> items = new Repository<Item>().GetAll() as List<Item>;
                Item anItem = items.Where(item => item.Id == order.ItemId).SingleOrDefault();
                return dbContext.Restaurants.Find(anItem.RestaurantId);
            }
            public bool UpdateAddress(int id, RestaurantAddress address)
            {
                Restaurant rest = dbContext.Restaurants.Find(id);

                RestaurantAddress restAdd = dbContext.RestaurantAddresses.Where(addr => addr.RestaurantId == id).SingleOrDefault();
                restAdd.Latitude = address.Latitude;
                restAdd.Longitude = address.Longitude;
                restAdd.FormattedAddress = address.FormattedAddress;

                RestaurantAddressRepositroy restAddRepo = new RestaurantAddressRepositroy();

                return restAddRepo.Update(restAdd, restAdd.Id);
            }
            public bool UpdateRating(int id,double rating)
            {
                Restaurant rest = dbContext.Restaurants.Find(id);
                double totalRating = 0;
                int count = 0;
                foreach (Review rev in dbContext.Reviews.Where(rev => rev.RestaurantId == id))
                {
                    totalRating += rev.Rating;
                    count++;
                }
                rest.Rating = (totalRating + rating) / (count + 1);
                this.Update(rest, id);
                return true;
            }
        }
}
