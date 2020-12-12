using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Common.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        [Required]
        [Display(Name = "Country")]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string ImageCountry { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath => string.IsNullOrEmpty(ImageCountry)
            ? $"https://localhost:44352/images/noimage.png"
            : $"https://localhost:44352/images/countries/{ImageCountry}";

        public ICollection<Department> Departments { get; set; }

        public int DepartmentsNumber => Departments == null ? 0 : Departments.Count;
    }
}
