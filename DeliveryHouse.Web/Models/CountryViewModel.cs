using DeliveryHouse.Common.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Web.Models
{
    public class CountryViewModel : Country
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
