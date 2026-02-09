using Microsoft.EntityFrameworkCore;
using VideoGameApi.Api.Interfaces.DigitalProducts;
using VideoGameApi.Data;
using VideoGameApi.Models.DigitalProducts;

namespace VideoGameApi.Api.Repositories.DigitalProducts
{
    public class DigitalProductKeyRepository : IDigitalProductKeyRepository
    {
        private readonly VideoGameDbContext _context;

        public DigitalProductKeyRepository(VideoGameDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DigitalProductKey key)
        {
            await _context.DigitalProductKeys.AddAsync(key);
        }

        public async Task<DigitalProductKey?> GetUnusedKeyAsync(int productId)
        {
            return await _context.DigitalProductKeys
                .Where(k => k.DigitalProductId == productId && !k.IsUsed)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DigitalProductKey>> GetKeysByProductIdAsync(int productId)
        {
            return await _context.DigitalProductKeys
                .Where(k => k.DigitalProductId == productId)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountUnusedKeyAsync(int digitalProductId)
        {
            return await _context.DigitalProductKeys
                 .CountAsync(k => k.DigitalProductId == digitalProductId && !k.IsUsed);
        }

        public Task MarkAsUsedAsync(DigitalProductKey key)
        {
            key.IsUsed = true;
            key.UsedAt = DateTime.UtcNow;
            _context.DigitalProductKeys.Update(key);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
