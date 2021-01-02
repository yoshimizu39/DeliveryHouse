using DeliveryHouse.Common.Entities;
using DeliveryHouse.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryHouse.Web.Data.Entities
{
    public class Order
    {
        public int id { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime? DateSend { get; set; }
        public DateTime? DateConfirmed { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public int Lines => OrderDetails == null ? 0 : OrderDetails.Count();
        public float Quantity => OrderDetails == null ? 0 : OrderDetails.Sum(od => od.Quantity);
        public decimal Value => OrderDetails == null ? 0 : OrderDetails.Sum(od => od.Value);

    }
}
