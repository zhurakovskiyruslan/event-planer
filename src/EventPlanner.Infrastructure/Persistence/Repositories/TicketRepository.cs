using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Data;
using EventPlanner.Data.Entities;

namespace EventPlanner.Infrastructure.Persistence.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly MyDbContext _context;

    public TicketRepository(MyDbContext context) => _context = context;
    
    public async Task<bool> ExistsAsync(int id)
        => await _context.Tickets.AnyAsync(t => t.Id == id);


    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Event)       // подтягиваем Event
            .Include(t => t.Bookings)    // подтягиваем связанные Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Ticket>> GetByEventIdAsync(int eventId)
    {
        return await _context.Tickets
            .Where(t => t.EventId == eventId)
            .Include(t => t.Bookings)    // полезно для связанных броней
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Ticket entity)
    {
        await _context.Tickets.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ticket entity)
    {
        _context.Tickets.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket is null) return;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }
}