using System;
using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.DigitalProducts;

namespace VideoGameApi.Models.DigitalOrders
{
    public class DigitalOrderItem
    {
        [Key]
        public int Id { get; set; }

        public int DigitalOrderId { get; set; }
        public DigitalOrder DigitalOrder { get; set; } = null!;

        public int DigitalProductKeyId { get; set; }
        public DigitalProductKey DigitalProductKey { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
