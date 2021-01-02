using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DeliveryHouse.Common.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int IdCountry { get; set; }

        [MaxLength(50, ErrorMessage = "The filed {0} must contain less than {1} characteres.")]
        [Required]
        [Display(Name = "Department")]
        public string Name { get; set; }

        [JsonIgnore]
        public Country Country { get; set; }

        public ICollection<City> Cities { get; set; }

        [Display(Name = "Cities Number")]
        public int CitiesNumber => Cities == null ? 0 : Cities.Count;
    }
}
