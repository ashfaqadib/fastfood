using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class EditProfileModel
    {
        [Required(ErrorMessage = "Please select a name")]
        [RegularExpression("^[a-zA-Z][a-zA-Z]* [a-zA-Z][a-zA-Z]*$", ErrorMessage = "Name must be at least two words")]
        public string Name { get; set; }

        [Required]
        public string Gender { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a valid day")]
        public string DoBDay { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a valid month")]
        public string DoBMonth { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a valid year")]
        public string DoBYear { get; set; }

        public string Email { get; set; }

        public string DateofBirth { get { return (DoBDay + "/" + DoBMonth + "/" + DoBYear); } set { value = DoBDay + "/" + DoBMonth + "/" + DoBYear; } }

        public string Role { get; set; }

        public string Status { get; set; }
    }
}