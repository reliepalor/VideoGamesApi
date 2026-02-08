using System.Collections.Generic;

namespace VideoGameApi.Api.Dto.Orders
{
    public class CheckoutDto
    {
        public List<int> CartItemIds { get; set; } = new();
    }
}
    