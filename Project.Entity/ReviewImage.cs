using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Entity
{
    public class ReviewImage
    {
        public int Id { get; set; }
        public string ImageLocation { get; set; }
        public int ReviewId { get; set; }
    }
}
