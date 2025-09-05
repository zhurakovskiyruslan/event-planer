using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Data;
using Microsoft.EntityFrameworkCore;
using EventEntity = EventPlanner.Data.Entities.Event;

namespace EventPlanner.Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly MyDbContext _context;

        public EventRepository(MyDbContext context) => _context = context;
        
        public async Task<bool> ExistsAsync(int id)
            => await _context.Events.AnyAsync(e => e.Id == id);


        public async Task<EventEntity?> GetByIdAsync(int id) =>
            await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == id);

        public async Task<List<EventEntity>> GetAllAsync() =>
            await _context.Events
                .Include(e => e.Location)
                .Include(e => e.Tickets)
                .ToListAsync();

        public async Task<List<EventEntity>> GetUpcomingEventsAsync() =>
            await _context.Events
                .Where(e => e.StartAtUtc > DateTime.UtcNow)
                .Include(e => e.Location)
                .Include(e => e.Tickets)
                .ToListAsync();

        public async Task AddAsync(EventEntity entity)
        {
            await _context.Events.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EventEntity entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Events.FindAsync(id);
            if (entity is null) return;

            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}