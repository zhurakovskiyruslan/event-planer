using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Data;              // MyDbContext
using EventPlanner.Data.Entities;     // User, Booking, Ticket, Event

namespace EventPlanner.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MyDbContext _context;

    public UserRepository(MyDbContext context) => _context = context;
    
    public async Task<bool> ExistsAsync(int id)
        => await _context.Users.AnyAsync(u => u.Id == id);


    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Bookings)
            .ThenInclude(b => b.Ticket)
            .ThenInclude(t => t.Event)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (existing is null) return;

        _context.Users.Remove(existing);
        await _context.SaveChangesAsync();
    }
}