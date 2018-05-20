using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Entity
{
    public class User
    {
        [Key]
        public string Email { get; set; }

        public string Password { get; set; }

        public int Id { get; set; }

        public string Role { get; set; }
    }
}
