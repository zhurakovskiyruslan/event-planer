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
            var entityToUpdate = await _context.Locations.FindAsync(entity.Id);
            if (entityToUpdate != null)
            {
                entityToUpdate.Name = entity.Name;
                entityToUpdate.Address = entity.Address;
                entityToUpdate.Capacity = entity.Capacity;

                _context.Locations.Update(entityToUpdate);
                await _context.SaveChangesAsync();
            }
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