using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Core.Interfaces
{
    public interface IPublicService
    {
        List<Review> GetAllReviews();
        List<Comment> GetReviewComments(int revId);
        string GetReviewerName(int custId);
        string GetCommenterName(int custId);
        List<string> GetReviewImageLocation(int revId);
    }
}
