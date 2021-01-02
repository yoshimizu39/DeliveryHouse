using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DeliveryHouse.Common.Entities
{
    public class OrderDetails
    {
        public int id { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }

        public float Quantity { get; set; }
        
        public decimal Price { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }
        
        public decimal Value => (decimal)Quantity * Price;
    }
}
