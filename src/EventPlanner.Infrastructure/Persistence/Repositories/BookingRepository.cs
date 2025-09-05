using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Data;              // MyDbContext
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums; // Booking, Ticket, Event, User

namespace EventPlanner.Infrastructure.Persistence.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly MyDbContext _context;

        public BookingRepository(MyDbContext context) => _context = context;
        
        public async Task<bool> ExistsAsync(int id)
            => await _context.Bookings.AnyAsync(b => b.Id == id);

        public async Task<Booking?> GetByIdAsync(int id) =>
            await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Ticket)
                    .ThenInclude(t => t.Event)
                        .ThenInclude(e => e.Location)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<List<Booking>> GetByUserIdAsync(int userId) =>
            await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Ticket)
                    .ThenInclude(t => t.Event)
                        .ThenInclude(e => e.Location)
                .Where(b => b.UserId == userId)
                .ToListAsync();

        public async Task<List<Booking>> GetByEventIdAsync(int eventId) =>
            await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Ticket)
                    .ThenInclude(t => t.Event)
                .Where(b => b.Ticket.EventId == eventId)
                .ToListAsync();

        public async Task<List<Booking>> GetActiveBookingsAsync() =>
            await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Ticket)
                    .ThenInclude(t => t.Event)
                .Where(b => b.Status == BookingStatus.Active)
                .ToListAsync();

        public Task<Booking?> GetByUserAndTicketAsync(int userId, int ticketId)
            => _context.Bookings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == userId
                                          && b.TicketId == ticketId);
        public async Task AddAsync(Booking entity)
        {
            _context.Bookings.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking entity)
        {
            _context.Bookings.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        public async Task CancelAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return;

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
        }
    }
}