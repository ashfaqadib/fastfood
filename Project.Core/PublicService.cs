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
    public class PublicService:IPublicService
    {
        ReviewRepository revRepo;
        public PublicService()
        {
            revRepo = new ReviewRepository();
        }
        public List<Review> GetAllReviews()
        {
            return revRepo.GetAll().ToList();
        }
        public string GetReviewerName(int custId)
        {
            return revRepo.GetReviewerName(custId);
        }
        public List<Comment> GetReviewComments(int revId)
        {
            return revRepo.GetComments(revId);
        }
        public string GetCommenterName(int custId)
        {
            return revRepo.GetCommenterName(custId);
        }
        public List<string> GetReviewImageLocation(int revId)
        {
            return revRepo.GetReviewImageLocation(revId);
        }
        
        
    }
}
