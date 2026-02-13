using VideoGameApi.Models.DigitalProducts; 

namespace VideoGameApi.Api.Interfaces.DigitalProducts
{
    public interface IDigitalProductKeyRepository
    {
        Task AddAsync(DigitalProductKey key);
        Task<IEnumerable<DigitalProductKey>> GetKeysByProductIdAsync(int digitalProductId);
        Task<DigitalProductKey?> GetUnusedKeyAsync(int digitalProductId);
        Task MarkAsUsedAsync(DigitalProductKey key);
        Task<int> CountUnusedKeyAsync(int digitalProductId);
        Task<IEnumerable<DigitalProductKey>> GetByProductIdAsync(int productId);
        Task SaveChangesAsync();
    }
}
