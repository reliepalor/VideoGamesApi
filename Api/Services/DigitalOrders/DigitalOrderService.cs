using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoGameApi.Api.Dto.DigitalOrders;
using VideoGameApi.Api.Interfaces.DigitalOrders;
using VideoGameApi.Api.Interfaces.DigitalProducts;
using VideoGameApi.Models.DigitalProducts;
using VideoGameApi.Models.DigitalOrders;
using VideoGameApi.Models.Orders;

namespace VideoGameApi.Api.Services.DigitalOrders
{
    public class DigitalOrderService : IDigitalOrderService
    {
        private readonly IDigitalOrderRepository _orderRepo;
        private readonly IDigitalOrderItemRepository _itemRepo;
        private readonly IDigitalProductRepository _productRepo;
        private readonly IDigitalProductKeyRepository _keyRepo;

        public DigitalOrderService(
            IDigitalOrderRepository orderRepo,
            IDigitalOrderItemRepository itemRepo,
            IDigitalProductRepository productRepo,
            IDigitalProductKeyRepository keyRepo)
        {
            _orderRepo = orderRepo;
            _itemRepo = itemRepo;
            _productRepo = productRepo;
            _keyRepo = keyRepo;
        }

        public async Task<int> CreateOrderAsync(int userId, CreateDigitalOrderDto dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.DigitalProductId);
            if (product == null || !product.IsActive)
                throw new InvalidOperationException("Product not found or inactive.");

            if (product.Stock < dto.Quantity)
                throw new InvalidOperationException("Insufficient stock.");

            var order = new DigitalOrder
            {
                UserId = userId,
                DigitalProductId = dto.DigitalProductId,
                Quantity = dto.Quantity,
                TotalPrice = product.Price * dto.Quantity,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
            };

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            // Optionally add order items here or after approval depending on logic

            return order.Id;
        }

        public async Task<IEnumerable<DigitalOrderResponseDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepo.GetByUserIdAsync(userId);
            return orders.Select(o => MapToDto(o));
        }

        public async Task<IEnumerable<DigitalOrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepo.GetAllAsync();
            return orders.Select(o => MapToDto(o));
        }

        public async Task<DigitalOrderResponseDto?> GetOrderByIdAsync(int id, int userId, bool isAdmin)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return null;

            if (!isAdmin && order.UserId != userId)
                throw new UnauthorizedAccessException();

            return MapToDto(order);
        }

        public async Task<bool> ApproveOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            var product = await _productRepo.GetByIdAsync(order.DigitalProductId);
            if (product == null || product.Stock < order.Quantity)
                throw new InvalidOperationException("Insufficient stock or product missing.");

            // Deduct stock
            product.Stock -= order.Quantity;
            await _productRepo.SaveChangesAsync();

            // Assign keys
            for (int i = 0; i < order.Quantity; i++)
            {
                var key = await _keyRepo.GetUnusedKeyAsync(order.DigitalProductId);
                if (key == null)
                    throw new InvalidOperationException("Not enough product keys.");

                key.IsUsed = true;
                key.AssignedToUserId = order.UserId;
                key.UsedAt = DateTime.UtcNow;

                await _keyRepo.MarkAsUsedAsync(key);

                var orderItem = new DigitalOrderItem
                {
                    DigitalOrderId = order.Id,
                    DigitalProductKeyId = key.Id,
                };
                await _itemRepo.AddAsync(orderItem);
            }

            order.Status = "Approved";
            order.ApprovedAt = DateTime.UtcNow;

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
            await _keyRepo.SaveChangesAsync();
            await _itemRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Status = "Rejected";
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
            return true;
        }

        private DigitalOrderResponseDto MapToDto(DigitalOrder order)
        {
            return new DigitalOrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                DigitalProductId = order.DigitalProductId,
                DigitalProductName = order.DigitalProduct?.Name ?? "",
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ApprovedAt = order.ApprovedAt,
                Items = order.Items?.Select(i => new DigitalOrderItemDto
                {
                    Id = i.Id,
                    ProductKey = i.DigitalProductKey?.ProductKey ?? "",
                    IsAssigned = i.DigitalProductKey != null && i.DigitalProductKey.IsUsed,
                    AssignedAt = i.DigitalProductKey?.UsedAt
                }).ToList() ?? new List<DigitalOrderItemDto>()
            };
        }
    }
}
