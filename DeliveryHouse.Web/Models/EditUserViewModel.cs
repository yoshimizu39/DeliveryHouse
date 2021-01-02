using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 6)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 6)]
        [Required]
        public string LastName { get; set; }

        [StringLength(100, MinimumLength = 10)]
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Display(Name = "Image")]
        public string ImageUser { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath => string.IsNullOrEmpty(ImageUser)
            ? $"https://localhost:44352/images/noimage.png"
            : $"https://localhost:44352/images/users/{ImageUser}";

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        [Required]
        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a contry")]
        public int IdCountry { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        [Required]
        [Display(Name = "Department")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a department")]
        public int IdDepartment { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }

        [Required]
        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city")]
        public int IdCity { get; set; }

        public IEnumerable<SelectListItem> Cities { get; set; }
    }
}
