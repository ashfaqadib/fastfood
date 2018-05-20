using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class CommentModel
    {
        public int ReviewId { get; set; }
        public string CommentText { get; set; }
        public string Time { get; set; }
        public string Commenter { get; set; } 
    }
}