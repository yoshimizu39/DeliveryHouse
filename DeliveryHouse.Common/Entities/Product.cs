using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeliveryHouse.Common.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdStore { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdCategory { get; set; }

        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Is Starred")]
        public bool IsStarred { get; set; }

        [Display(Name = "Image")]
        public string ImageProduct { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(ImageProduct)
            ? $"https://localhost:44352/images/noimage.png"
            : $"https://localhost:44352/images/products/{ImageProduct}";


        [JsonIgnore]
        public Store Store { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }
    }
}
