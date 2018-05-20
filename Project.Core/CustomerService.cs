using Project.Core.Interfaces;
using Project.Data;
using Project.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core
{
    public class CustomerService:Service<Customer>,ICustomerService
    {
        CustomerRepository custrepo;

        public CustomerService()
        {
            custrepo = new CustomerRepository();
        }

        public List<Order> AddToCart(int itemId, List<Order> Orders)
        {
            Item item = new Repository<Item>().Get(System.Convert.ToInt32(itemId));
            Order order = Orders.Where(order1 => order1.ItemId == item.Id).SingleOrDefault();
            if (order == null)
            {
                order = new Order();
                order.ItemId = item.Id;
                order.ItemName = item.Name;
                order.Total = item.Price;
                order.Quantity = 1;
                Orders.Add(order);
            }
            else
            {
                order.Quantity++;
                order.Total = order.Quantity * item.Price;
            }
            return Orders;
        }

        public List<Order> RemoveFromCart(int itemId, List<Order> Orders)
        {
            Order order = Orders.Where(order1 => order1.ItemId == System.Convert.ToInt32(itemId)).SingleOrDefault();

            if (order.Quantity > 1)
            {
                order.Total = order.Total / order.Quantity;
                order.Quantity--;
                order.Total = order.Total * order.Quantity;
            }
            else
            {
                Orders.Remove(order);
            }
            return Orders;
        }

        public Customer UpdateDeliveryInfo(Customer cust, string newAddress,string contactNo)
        {
            cust.DeliveryAddress = newAddress;
            cust.ContactNumber = contactNo;
            new Repository<Customer>().Update(cust, cust.Id);
            return cust;
        }

        public void CheckOut(Customer cust, List<Order> Orders)
        {
            double bill = 0;
            Repository<Order> repo = new Repository<Order>();
            int invId = repo.dbContext.Invoices.Count() + 1;

            foreach (Order order in Orders)
            {
                order.InvoiceId = invId;
                repo.Insert(order);
                bill += order.Total;
            }

            Restaurant rest = new RestaurantRepository().GetByOrder(Orders.First());
            Invoice inv = new InvoiceRepository().PrepareInvoice(cust,rest,bill);

            Repository<Invoice> repo2 = new Repository<Invoice>();
            repo2.Insert(inv);
        }

        public double CartBill(List<Order> Orders)
        {
            double bill = 0;
            foreach (Order ord in Orders)
            {
                bill += ord.Total;
            }
            return bill;
        }

        public int PostReview(Review review,int custId)
        {
            review.Time = DateTime.Now;
            review.CustomerId = custId;
            new RestaurantRepository().UpdateRating(review.RestaurantId,review.Rating);
            custrepo.dbContext.Reviews.Add(review);
            return custrepo.dbContext.SaveChanges();
        }
        public int GetImageId()
        {
            return custrepo.dbContext.ReviewImages.Count() + 1;
        }
        public int AddReviewImage(string path)
        {
            ReviewImage revImage = new ReviewImage();
            revImage.ImageLocation = path;
            revImage.ReviewId = custrepo.dbContext.Reviews.Count();
            custrepo.dbContext.ReviewImages.Add(revImage);
            return custrepo.dbContext.SaveChanges();
        }
        public List<Review> GetAllReviews()
        {
            return custrepo.dbContext.Reviews.ToList();
        }
        public List<string> GetReviewImageLocation(int reviewId)
        {
            return custrepo.dbContext.ReviewImages.Where(rev => rev.ReviewId == reviewId).Select(rev=> rev.ImageLocation).ToList();
        }
        public List<Comment> GetAllComments(int reviewId)
        {
            return custrepo.dbContext.Comments.Where(rev => rev.ReviewId == reviewId).ToList();
        }
        public int AddComment(Comment comment)
        {
            custrepo.dbContext.Comments.Add(comment);
            return custrepo.dbContext.SaveChanges();
        }
        public bool Delete(Customer custodelete)
        {
            return custrepo.Delete(custodelete);
        }
    }
}
