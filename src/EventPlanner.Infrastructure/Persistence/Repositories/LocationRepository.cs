using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Data;
using EventPlanner.Data.Entities;

namespace EventPlanner.Infrastructure.Persistence.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly MyDbContext _context;

        public LocationRepository(MyDbContext context) => _context = context;
        
        public async Task<bool> ExistsAsync(int id)
            => await _context.Locations.AnyAsync(l => l.Id == id);

        public async Task<Location?> GetByIdAsync(int id)
        {
            // Если в Location есть навигация на события (Events),
            // можно добавить Include(l => l.Events)
            return await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Location entity)
        {
            await _context.Locations.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Location entity)
        {
            // Если entity не отслеживается — Attach + Modified
            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Locations.FirstOrDefaultAsync(l => l.Id == id);
            if (existing is null) return;

            _context.Locations.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}