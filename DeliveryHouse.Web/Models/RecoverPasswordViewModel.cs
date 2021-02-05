using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
