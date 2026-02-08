using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VideoGameApi.Api.Interfaces.DigitalOrders;
using VideoGameApi.Data;
using VideoGameApi.Models.DigitalOrders;
using VideoGameApi.Models.DigitalProducts;

namespace VideoGameApi.Api.Repositories.DigitalOrders
{
    public class DigitalOrderItemRepository : IDigitalOrderItemRepository
    {
        private readonly VideoGameDbContext _context;

        public DigitalOrderItemRepository(VideoGameDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(DigitalOrderItem item)
        {
            await _context.DigitalOrderItems.AddAsync(item);
        }

        public async Task<DigitalOrderItem?> GetByIdAsync(int id)
        {
            return await _context.DigitalOrderItems
                .Include(i => i.DigitalProductKey)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(DigitalOrderItem item)
        {
            _context.DigitalOrderItems.Update(item);
        }
    }
}
