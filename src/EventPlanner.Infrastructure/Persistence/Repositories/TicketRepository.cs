using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.ReadModels;
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

    public async Task<List<TicketDto>> GetByEventIdAsync(int eventId)
    {
        return await _context.Tickets
            .Where(t => t.EventId == eventId)
            .Include(t => t.Bookings)    // полезно для связанных броней
            .Include(t => t.Event)
            .ThenInclude(e => e.Location)
            .Select(t => new TicketDto(
                t.Id,
                t.Type,
                t.Price,
                t.EventId,
                t.Event.Title,
                t.Event.Description,
                t.Event.StartAtUtc,
                t.Event.Location.Name,
                t.Event.Location.Address
                ))
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
        var ticketToUpdate = await _context.Tickets.FindAsync(entity.Id);
        if (ticketToUpdate != null)
        {
            ticketToUpdate.Type = entity.Type;
            ticketToUpdate.Price = entity.Price;
            ticketToUpdate.EventId = entity.EventId;
            _context.Tickets.Update(ticketToUpdate);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket is null) return;

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
    }
}