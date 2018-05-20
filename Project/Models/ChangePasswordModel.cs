using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string CurrentPassword { set; get; }

        [Required]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; }

        [Required]
        [CompareAttribute("NewPassword", ErrorMessage = "Password not matched!")]
        public string RetypePassword { get; set; }
    }
}