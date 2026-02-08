using VideoGameApi.Models.DigitalOrders;
using VideoGameApi.Models.DigitalProducts;

namespace VideoGameApi.Api.Interfaces.DigitalOrders
{
    public interface IDigitalOrderItemRepository
    {
        Task AddAsync(DigitalOrderItem item);
        Task<DigitalOrderItem?> GetByIdAsync(int id);
        Task SaveChangesAsync();
        void Update(DigitalOrderItem item);
    }
}