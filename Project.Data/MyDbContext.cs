using Project.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class MyDbContext:DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewImage> ReviewImages { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<RestaurantAddress> RestaurantAddresses { get; set; }
        public DbSet<Transporter> Transporters { get; set; }
        public DbSet<TransporterTask> TransporterTasks { get; set; }
        private static MyDbContext DbContextInstance;

        private MyDbContext()
            : base("TestDb")
        {
            Database.SetInitializer<MyDbContext>(new DropCreateDatabaseIfModelChanges<MyDbContext>());
        }

        public static MyDbContext GetDbContext ()
        {
            if (DbContextInstance == null)
            {
                DbContextInstance = new MyDbContext();
                return DbContextInstance;
            }
            else return DbContextInstance;
        }

        public void AddToDb()
        {
            List<User> userEntry = new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Role = "Customer",
                    Password = "123",
                    Email = "obama@aiub.edu"
                },
                new User()
                {
                    Id = 2,
                    Role = "Customer",
                    Password = "123",
                    Email = "cust"
                },
                new User()
                {
                    Id = 3,
                    Role = "Admin",
                    Password = "123",
                    Email = "admin"
                },
                new User()
                {
                    Id = 4,
                    Role = "Restaurant",
                    Password = "123",
                    Email = "rest"
                },
                new User()
                {
                    Id = 5,
                    Role = "Restaurant",
                    Password = "123",
                    Email = "burgerking@gmail.com"
                },
                new User()
                {
                    Id = 6,
                    Role = "Restaurant",
                    Password = "123",
                    Email = "chatime@gmail.com"
                },
                new User()
                {
                    Id = 7,
                    Role = "Restaurant",
                    Password = "123",
                    Email = "madchef@gmail.com"
                },
                new User()
                {
                    Id = 8,
                    Role = "Restaurant",
                    Password = "123",
                    Email = "starkabab@gmail.com"
                },
                new User()
                {
                    Id = 9,
                    Role = "Transporter",
                    Password = "123",
                    Email = "transporter"
                },
                 new User()
                {
                    Id = 10,
                    Role = "Transporter",
                    Email = "transporter2",
                    Password = "123"
                }
            };
            List<Customer> users = new List<Customer>()
            {
                new Customer()
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "obama@aiub.edu",
                    Password = "123",
                    Gender = "Male",
                    Status = "Active",
                    DateOfBirth = "13/07/2001",
                    LastOnline = new DateTime(2017, 3, 27)
                },
                new Customer()
                {
                    Id = 2,
                    Name = "John Martin",
                    Email = "cust",
                    Password = "123",
                    Gender = "Female",
                    Status = "Active",
                    DateOfBirth = "13/07/2001",
                    LastOnline = new DateTime(2017, 3, 27)
                }
            };
            List<Transporter> transporters = new List<Transporter>()
            {
                new Transporter()
                {
                    Id = 9,
                    Name = "Jason Smith",
                    Email = "transporter",
                    Password = "123",
                    Gender = "Male",
                    Status = "Active",
                    DateOfBirth = "13/07/2001",
                    ContactNumber = "123456789",
                    LastOnline = new DateTime(2017, 3, 27)
                },
                new Transporter()
                {
                    Id = 10,
                    Name = "Mary Fisher",
                    Email = "transporter2",
                    Password = "123",
                    Gender = "Female",
                    Status = "Active",
                    DateOfBirth = "13/07/2001",
                    ContactNumber = "123456789",
                    LastOnline = new DateTime(2017, 3, 27)
                }
            };
            Admin admin = new Admin()
            {
                Id = 3,
                Name = "Andrew Bernard",
                Email = "admin",
                Password = "123",
                Gender = "Female",
                Status = "Active",
                DateOfBirth = "13/07/2001",
                LastOnline = new DateTime(2017, 3, 27)
            };

            List<Restaurant> rest = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Id = 4,
                    Name = "Chef's Cuisine",
                    Email = "rest",
                    Password = "123",
                    Status = "Active",
                    OpenHours = "10:00 A.M. - 10 P.M.",
                    DiscountinPercentage = 0,
                    VATinPercentage = 15,
                    DeliveryCharge =75,
                    LastOnline = new DateTime(2017, 3, 27),
                    ImageLocation = "..//image/restaurants/takeout.jpg",
                    AddressId = 1,
                    MinimumOrder = 200
                },
                               
                new Restaurant()
                {
                    Id = 5,
                    ImageLocation="..//image/restaurants/burgerking.jpg",
                    OpenHours="12 A.M.-12 P.M.",
                    Password = "123",
                    LastOnline = new DateTime(2017, 3, 27),
                    AddressId = 2,
                    MinimumOrder=400,
                    Name = "Sbarro",
                    Email="burgerking@gmail.com",
                    DiscountinPercentage = 0,
                    VATinPercentage = 0,
                    DeliveryCharge =75,
                    Status="Pending"
                },
                new Restaurant()
                {
                    Id = 6,
                    ImageLocation="..//image/restaurants/madchef.jpg",
                    OpenHours="10 A.M.-9 P.M.",
                    Password = "123",
                    LastOnline = new DateTime(2017, 3, 27),
                    AddressId = 3,
                    MinimumOrder=250,
                    DiscountinPercentage = 0,
                    VATinPercentage = 0,
                    DeliveryCharge =75,
                    Name = "Baluchor",
                    Email="chatime@gmail.com",
                    Status="Active"
                },
                new Restaurant()
                {
                    Id = 7,
                    ImageLocation="..//image/restaurants/madchef.jpg",
                    OpenHours="10 A.M.-9 P.M.",
                    Password = "123",
                    LastOnline = new DateTime(2017, 3, 27),
                    AddressId = 4,
                    MinimumOrder=250,
                    Name = "Madchef",
                    DiscountinPercentage = 0,
                    VATinPercentage = 0,
                    DeliveryCharge =75,
                    Email="madchef@gmail.com",
                    Status="Active"
                },
                new Restaurant()
                {
                    Id = 8,
                    ImageLocation="..//image/restaurants/starkabab.jpg",
                    OpenHours="8 A.M.-10 P.M.",
                    Password = "123",
                    LastOnline = new DateTime(2017, 3, 27),
                    AddressId = 5,
                    MinimumOrder=200,
                    Name = "Star Kabab",
                    DiscountinPercentage = 0,
                    VATinPercentage = 0,
                    DeliveryCharge =75,
                    Email="starkabab@gmail.com",
                    Status="Blocked"
                },
                new Restaurant()
                {
                    Id = 9,
                    ImageLocation="..//image/restaurants/madchef.jpg",
                    OpenHours="10 A.M.-9 P.M.",
                    Password = "123",
                    LastOnline = new DateTime(2017, 3, 27),
                    AddressId = 6,
                    MinimumOrder=250,
                    Name = "CDA",
                    DiscountinPercentage = 0,
                    VATinPercentage = 0,
                    DeliveryCharge =75,
                    Email="ctgchatime@gmail.com",
                    Status="Active"
                }
            };

            List<RestaurantAddress> restAdds = new List<RestaurantAddress>()
            {
                new RestaurantAddress()
                {
                    Latitude = 23.771312,
                    Longitude = 90.363925,
                    FormattedAddress = "Shyamoli",
                    RestaurantId = 4
                },
                new RestaurantAddress()
                {
                    Latitude = 23.765444,
                    Longitude = 90.359648,
                    FormattedAddress = "Mohammadpur",
                    RestaurantId = 5
                },               
                new RestaurantAddress()
                {
                    Latitude = 24.905083,
                    Longitude = 91.895732,
                    FormattedAddress = "Baluchor",
                    RestaurantId = 6
                },               
                new RestaurantAddress()
                {
                    Latitude = 23.800149,
                    Longitude = 90.420723,
                    FormattedAddress = "Ajimpur",
                    RestaurantId = 7
                },                
                new RestaurantAddress()
                {
                    Latitude = 23.733282,
                    Longitude = 90.384546,
                    FormattedAddress = "Shyamoli",
                    RestaurantId = 8
                },                
                new RestaurantAddress()
                {
                    Latitude = 22.334131,
                    Longitude = 91.835017,
                    FormattedAddress = "CDA",
                    RestaurantId = 9
                }

            };

            List<Item> items = new List<Item>()
            {
                new Item()
                {
                    Name = "Chicken Burger",
                    Description = "Chicken burger it is.",
                    Type = "Burger",
                    Price = 120,
                    RestaurantId = 4,
                    Proportion = "1:1"
                },
                new Item()
                {
                    Name = "Beef Burger",
                    Description = "Chicken burger it is.",
                    Type = "Burger",
                    Price = 120,
                    RestaurantId = 4,
                    Proportion = "1:1"
                },
                new Item()
                {
                    Name = "Beef Cheese Burger",
                    Description = "Chicken burger it is.",
                    Type = "Burger",
                    Price = 120,
                    RestaurantId = 4,
                    Proportion = "1:1"
                },
                new Item()
                {
                    Name = "Chicken Sandwich",
                    Description = "Chicken sandwich with double bread.",
                    Type = "Sandwich",
                    Price = 80,
                    RestaurantId = 4,
                    Proportion = "1:1"
                },
                new Item()
                {
                    Name = "Supreme Pizza",
                    Description = "Pizza with chicken,beef,pepperoni,mushroom and everything else.",
                    Type = "Pizza",
                    Price = 1850,
                    RestaurantId = 5,
                    Proportion = "12\""
                },
                new Item()
                {
                    Name = "Supreme Pizza",
                    Description = "Pizza with chicken,beef,pepperoni,mushroom and everything else.",
                    Type = "Pizza",
                    Price = 1850,
                    RestaurantId = 6,
                    Proportion = "12\""
                }
            };

            Invoice invoices = new Invoice();

            List<Order> orders = new List<Order>()
            {
                new Order(){
                ItemId = 1,
                ItemName = "Chicken Burger",
                InvoiceId = 1
                },                
                new Order(){
                ItemId = 2,
                ItemName = "Beef Burger",
                InvoiceId = 1
                },
                new Order(){
                ItemId = 3,
                ItemName = "Beef Cheese Burger",
                InvoiceId = 1
                }

            };

            invoices.RestaurantId = 4;
            invoices.CheckOutTime = DateTime.Now;
            invoices.CustomerId = 2;
            invoices.Bill = 1200;

            this.Invoices.Add(invoices);

            foreach (Order user in orders)
            {
                this.Orders.Add(user);
            }

            foreach (Customer user in users)
            {
                this.Customers.Add(user);
            }

            foreach (Customer user in users)
            {
                this.Customers.Add(user);
            }

            foreach (Item item in items)
            {
                this.Items.Add(item);
            }
            foreach (User user2 in userEntry)
            {
                this.Users.Add(user2);
            }
            foreach (Restaurant rests in rest)
            {
                this.Restaurants.Add(rests);
            }
            foreach (RestaurantAddress adds in restAdds)
            {
                this.RestaurantAddresses.Add(adds);
            }
            foreach (Transporter trs in transporters)
            {
                this.Transporters.Add(trs);
            }
            this.Admins.Add(admin);
            this.SaveChanges();
        }
    }
}
