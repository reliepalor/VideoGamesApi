using System.Collections.Generic;
using System.Threading.Tasks;
using VideoGameApi.Api.Dto.DigitalOrders;

namespace VideoGameApi.Api.Interfaces.DigitalOrders
{
    public interface IDigitalOrderService
    {
        Task<(bool Success, int OrderId, string? Error)> CreateOrderAsync(int userId, CreateDigitalOrderDto dto);
        Task<IEnumerable<DigitalOrderResponseDto>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<DigitalOrderResponseDto>> GetAllOrdersAsync();
        Task<DigitalOrderResponseDto?> GetOrderByIdAsync(int id, int userId, bool isAdmin);
        Task<bool> ApproveOrderAsync(int orderId);
        Task<bool> RejectOrderAsync(int orderId);
    }
}
