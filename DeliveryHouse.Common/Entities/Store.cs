using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeliveryHouse.Common.Entities
{
    public class Store
    {
        public int Id { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdCity { get; set; }

        [StringLength(100)]
        [Required]
        [Display(Name = "Store")]
        public string Name { get; set; }

        [StringLength(100)]
        public string Direction { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Display(Name = "Image")]
        public string ImageStore { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath => string.IsNullOrEmpty(ImageStore)
    ? $"https://localhost:44352/images/noimage.png"
    : $"https://localhost:44352/images/stores/{ImageStore}";

        [JsonIgnore]
        public City City { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
