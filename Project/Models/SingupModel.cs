using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class SignupModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z][a-zA-Z]* [a-zA-Z][a-zA-Z]*$", ErrorMessage = "Name must be at least two words")]
        public string Name { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }

        [Required]
        [NotMapped]
        [CompareAttribute("Password", ErrorMessage = "Does not match with password!")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Gender { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a valid day")]
        public int DoBDay { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a valid month")]
        public int DoBMonth { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please select a valid year")]
        public int DoBYear { get; set; }

        public string DateofBirth { get { return (DoBDay + "/" + DoBMonth + "/" + DoBYear); } set { value = DoBDay + "/" + DoBMonth + "/" + DoBYear; } }
    }
}