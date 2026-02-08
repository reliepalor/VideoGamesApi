using System;
using System.Collections.Generic;

namespace VideoGameApi.Api.Dto.DigitalOrders
{
    public class DigitalOrderResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DigitalProductId { get; set; }
        public string DigitalProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<DigitalOrderItemDto> Items { get; set; } = new List<DigitalOrderItemDto>();
    }
}
