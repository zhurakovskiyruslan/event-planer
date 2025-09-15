using System.ComponentModel.DataAnnotations;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Data;
using EventPlanner.Data.Entities;
using Microsoft.EntityFrameworkCore;
using EventEntity = EventPlanner.Data.Entities.Event;
using EventPlanner.Application.ReadModels;

namespace EventPlanner.Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly MyDbContext _context;

        public EventRepository(MyDbContext context) => _context = context;
        
        public async Task<bool> ExistsAsync(int id)
            => await _context.Events.AnyAsync(e => e.Id == id);


        public async Task<EventDto?> GetByIdAsync(int id) =>
            await _context.Events
                .AsNoTracking()
                .Include(e=>e.Tickets)
                .Where(e => e.Id == id)                 
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.Description,
                    e.StartAtUtc,
                    e.Capacity,
                    e.Location.Name
                ))
                .FirstOrDefaultAsync();     

        public async Task<List<EventDto>> GetAllAsync() =>
            await _context.Events
                .Include(e => e.Location)
                .Select(e => new EventDto(
                    e.Id,
                    e.Title,
                    e.Description,
                    e.StartAtUtc,
                    e.Capacity,
                    e.Location.Name
                ))
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
            var entityToUpdate = await _context.Events.FindAsync(entity.Id);
            if (entityToUpdate != null)
            {
                entityToUpdate.Title = entity.Title;
                entityToUpdate.Description = entity.Description;
                entityToUpdate.StartAtUtc = entity.StartAtUtc;
                entityToUpdate.Capacity = entity.Capacity;
                entityToUpdate.LocationId = entity.LocationId;
                _context.Events.Update(entityToUpdate);
                await _context.SaveChangesAsync();
            }
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