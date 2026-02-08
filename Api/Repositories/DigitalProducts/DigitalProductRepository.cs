using Microsoft.EntityFrameworkCore;
using VideoGameApi.Api.Interfaces.DigitalProducts;
using VideoGameApi.Data;
using VideoGameApi.Models.DigitalProducts;

namespace VideoGameApi.Repositories.DigitalProducts
{
    public class DigitalProductRepository : IDigitalProductRepository
    {
        private readonly VideoGameDbContext _context;

        public DigitalProductRepository(VideoGameDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DigitalProduct product)
        {
            await _context.DigitalProducts.AddAsync(product);
        }

        public async Task DeleteAsync(DigitalProduct product)
        {
            _context.DigitalProducts.Remove(product);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.DigitalProducts.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<DigitalProduct>> GetAllAsync()
        {
            return await _context.DigitalProducts
                .Include(p => p.ProductKeys)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<DigitalProduct?> GetByIdAsync(int id)
        {
            return await _context.DigitalProducts
                .Include(p => p.ProductKeys)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistAsync(int id)
        { 
            return await _context.DigitalProducts.AnyAsync (p => p.Id == id);
        }

        public async Task UpdateAsync(DigitalProduct product)
        {
            _context.DigitalProducts.Update(product);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
