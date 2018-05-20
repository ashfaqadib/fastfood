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
    public class RestaurantAddressService:Service<RestaurantAddress>,IRestaurantAddressService
    {
        RestaurantAddressRepositroy restAddRepo;
        public RestaurantAddressService()
        {
            this.restAddRepo = new RestaurantAddressRepositroy();
        }
    }
}
