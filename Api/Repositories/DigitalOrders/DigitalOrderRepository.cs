using Microsoft.EntityFrameworkCore;
using VideoGameApi.Api.Interfaces.DigitalOrders;
using VideoGameApi.Data;
using VideoGameApi.Models.DigitalOrders;

namespace VideoGameApi.Api.Repositories.DigitalOrders
{
    public class DigitalOrderRepository : IDigitalOrderRepository
    {
        private readonly VideoGameDbContext _context;

        public DigitalOrderRepository(VideoGameDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DigitalOrder order)
        {
            await _context.DigitalOrders.AddAsync(order);
        }

        public async Task<IEnumerable<DigitalOrder>> GetAllAsync()
        {
            return await _context.DigitalOrders
                .Include(o => o.User)
                .Include(o => o.DigitalProduct)
                .Include(o => o.Items)
                    .ThenInclude(i => i.DigitalProductKey)
                .ToListAsync();
        }

        public async Task<DigitalOrder?> GetByIdAsync(int id)
        {
            return await _context.DigitalOrders
                .Include(o => o.User)
                .Include(o => o.DigitalProduct)
                .Include(o => o.Items)
                    .ThenInclude(i => i.DigitalProductKey)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<DigitalOrder>> GetByUserIdAsync(int userId)
        {
            return await _context.DigitalOrders
                .Where(o => o.UserId == userId)
                .Include(o => o.DigitalProduct)
                .Include(o => o.Items)
                    .ThenInclude(i => i.DigitalProductKey)
                .ToListAsync();
        }

        public void Update(DigitalOrder order)
        {
            _context.DigitalOrders.Attach(order);
            _context.Entry(order).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
