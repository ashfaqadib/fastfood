using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Data
{
    public class ReviewRepository:Repository<Review>
    {
        public string GetReviewerName(int custId)
        {
            return dbContext.Customers.Where(c => c.Id == custId).SingleOrDefault().Name;
        }
        public List<Comment> GetComments(int revId)
        {
            return dbContext.Comments.Where(rev => rev.ReviewId == revId).ToList();
        }
        public string GetCommenterName(int custId)
        {
            return dbContext.Customers.Where(c => c.Id == custId).SingleOrDefault().Name;
        }
        public List<string> GetReviewImageLocation(int reviewId)
        {
            return dbContext.ReviewImages.Where(rev => rev.ReviewId == reviewId).Select(rev => rev.ImageLocation).ToList();
        }
    }
}
