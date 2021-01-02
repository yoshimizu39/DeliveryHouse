using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeliveryHouse.Common.Entities
{
    public class City
    {
        public int Id { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdDepartment { get; set; }

        [MaxLength(50, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        [Required]
        [Display(Name = "City")]
        public string Name { get; set; }

        [JsonIgnore]
        public Department Department { get; set; }

        public ICollection<Store> Stores { get; set; }

        [Display(Name = "Stores Number")]
        public int StoresNumber => Stores == null ? 0 : Stores.Count;
    }
}
