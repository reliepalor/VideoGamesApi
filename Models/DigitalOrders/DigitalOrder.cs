using System;
using System.Collections.Generic;
using VideoGameApi.Models.DigitalProducts;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Models.DigitalOrders
{
    public class DigitalOrder
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public int DigitalProductId { get; set; }
        public DigitalProduct DigitalProduct { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }

        public ICollection<DigitalOrderItem> Items { get; set; } = new List<DigitalOrderItem>();
    }
}
