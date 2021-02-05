using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The file {0} filed must contain between {2} and {1} characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The file {0}  must contain between {2} and {1} characters.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
