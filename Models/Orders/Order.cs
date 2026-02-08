using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Models.Orders
{
    public enum OrderStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
