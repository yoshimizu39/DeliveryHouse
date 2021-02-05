using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryHouse.Common.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdStore { get; set; }

        [MaxLength(50, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string ImageCategory { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(ImageCategory)
        ? "http://deliveryhouse.somee.com/images/noimage.png"
        : $"http://deliveryhouse.somee.com/{ImageCategory}";

        [JsonIgnore]
        public Store Store { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
