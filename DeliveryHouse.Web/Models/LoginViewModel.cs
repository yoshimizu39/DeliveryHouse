using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Web.Models
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required]
        public string UserName { get; set; }

        [MaxLength(6)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Remeber Me")]
        public bool RememberMe { get; set; }
    }
}
