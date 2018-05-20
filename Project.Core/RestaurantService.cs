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
    public class RestaurantService:Service<Restaurant>,IRestaurantService
    {
        RestaurantRepository restRepo;
        public RestaurantService()
        {
            restRepo = new RestaurantRepository();
        }
        public bool UpdateLogo(int restaurantId,string imageLocation)
        {
            Restaurant rest = restRepo.Get(restaurantId);
            rest.ImageLocation = imageLocation;
            return restRepo.Update(rest, rest.Id);
        }
        public RestaurantAddress GetRestaurantAddress(int restId)
        {
            Restaurant thisRest = restRepo.Get(restId);

            RestaurantAddress address = restRepo.dbContext.RestaurantAddresses.Where(add=>add.RestaurantId==restId).SingleOrDefault();

            return address;
        }
        public bool Update(Restaurant editedRest,int restaurantId)
        {
            Restaurant thisUser = restRepo.Get(restaurantId);

            editedRest.Email = thisUser.Email;
            editedRest.Id = thisUser.Id;
            editedRest.Password = thisUser.Password;
            editedRest.LastOnline = DateTime.Now;
            editedRest.Status = thisUser.Status;
            editedRest.AddressId = thisUser.AddressId;
            editedRest.Rating = thisUser.Rating;
            if (editedRest.ImageLocation == null) editedRest.ImageLocation = thisUser.ImageLocation;

            return restRepo.Update(editedRest, thisUser.Id);
        }
        public Restaurant GetByInvoice(Invoice inv)
        {
            return restRepo.GetByInvoice(inv);
        }
        public Restaurant GetByOrder(Order order)
        {
            return restRepo.GetByOrder(order);
        }
        public bool UpdateAddress(int id, RestaurantAddress address)
        {
            return restRepo.UpdateAddress(id, address);
        }
        public bool Delete(Restaurant restodelete)
        {
            return restRepo.Delete(restodelete);
        }
    }
}
