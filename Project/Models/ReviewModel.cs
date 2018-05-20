using Project.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ReviewModel
    {
        public int ReviewId { get; set; }
        public int RestaurantId { get; set; }
        public string Reviewer { get; set; }
        public string RestaurantName { get; set; }
        public string Time { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }

        public List<Comment> Comments { get; set; }
        public List<CommentModel> CommentModels { get; set; }
        public List<string> Texts { get; set; }
        public List<string> Commenters { get; set; }
        public List<string> CommentTimes { get; set; }

        public ReviewModel()
        {
            this.Images = new List<string>();
            this.Commenters = new List<string>();
            this.Texts = new List<string>();
            this.Comments = new List<Comment>();
            this.CommentTimes = new List<string>();
            this.CommentModels = new List<CommentModel>();
        }
    }
}