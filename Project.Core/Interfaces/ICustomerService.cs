using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface ICustomerService:IService<Customer>
    {
        Customer UpdateDeliveryInfo(Customer cust, string newAddress, string contactNo);
        int PostReview(Review review, int custId);
        List<Review> GetAllReviews();
        List<Comment> GetAllComments(int reviewId);
        int AddComment(Comment comment);
        List<Order> AddToCart(int itemId, List<Order> Orders);
        List<Order> RemoveFromCart(int itemId, List<Order> Orders);
        void CheckOut(Customer cust, List<Order> Orders);
        int GetImageId();
        int AddReviewImage(string path);
        double CartBill(List<Order> Orders);
        List<string> GetReviewImageLocation(int reviewId);
        bool Delete(Customer custodelete);
    }
}
