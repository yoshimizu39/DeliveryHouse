using DeliveryHouse.Common.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Web.Models
{
    public class StoreViewModel : Store
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        //public ICollection<Category> Categoriess { get; set; }
    }
}
