using System.Collections.Generic;
using System.Threading.Tasks;
using VideoGameApi.Models.DigitalOrders;

namespace VideoGameApi.Api.Interfaces.DigitalOrders
{
    public interface IDigitalOrderRepository
    {
        Task AddAsync(DigitalOrder order);
        Task<DigitalOrder?> GetByIdAsync(int id);
        Task<IEnumerable<DigitalOrder>> GetAllAsync();
        Task<IEnumerable<DigitalOrder>> GetByUserIdAsync(int userId);
        void Update(DigitalOrder order);
        Task SaveChangesAsync();
    }
}
