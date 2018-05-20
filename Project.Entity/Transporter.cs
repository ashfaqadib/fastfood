using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Entity
{
    public class Transporter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string ContactNumber { get; set; }

        public string Password { get; set; }

        public string Gender { get; set; }

        public DateTime LastOnline { get; set; }

        public string DateOfBirth { get; set; }

        public string Status { get; set; }

        public string Residence { get; set; }

        public double Points { get; set; }
    }
}
