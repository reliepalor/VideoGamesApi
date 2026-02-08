using VideoGameApi.Models.DigitalProducts;

namespace VideoGameApi.Api.Interfaces.DigitalProducts
{
    public interface IDigitalProductRepository
    {
        Task<DigitalProduct?> GetByIdAsync(int id);
        Task<IEnumerable<DigitalProduct>> GetAllAsync();
        Task AddAsync(DigitalProduct product);
        Task UpdateAsync(DigitalProduct product);
        Task DeleteAsync(DigitalProduct product);
        Task<bool> ExistAsync(int id);
        Task SaveChangesAsync();
    }
}
