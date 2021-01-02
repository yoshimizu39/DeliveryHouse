using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Common.Request
{
    public class EmailRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
