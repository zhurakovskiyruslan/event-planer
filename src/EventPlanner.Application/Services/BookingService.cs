using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IUserRepository _userRepo;
    private readonly ITicketRepository _ticketRepo;

    public BookingService(
        IBookingRepository bookingRepo,
        IUserRepository userRepo,
        ITicketRepository ticketRepo)
    {
        _bookingRepo = bookingRepo;
        _userRepo = userRepo;
        _ticketRepo = ticketRepo;
    }

    /// <summary>Создать бронь.</summary>
    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _bookingRepo.AddAsync(booking);
        return booking;
    }

    /// <summary>Отмена брони. (Опционально проверяем владельца)</summary>
    public async Task CancelAsync(int bookingId, int? actorUserId = null)
    {
        var booking = await _bookingRepo.GetByIdAsync(bookingId)
                      ?? throw new InvalidOperationException("Бронь не найдена");

        if (actorUserId.HasValue && booking.UserId != actorUserId.Value)
            throw new InvalidOperationException("Нельзя отменить бронь другого пользователя");

        booking.Cancel(); // доменная логика внутри сущности
        await _bookingRepo.UpdateAsync(booking);
    }
    
    public Task DeleteAsync(int id) =>
        _bookingRepo.DeleteAsync(id);

    public Task<Booking?> GetById(int id) => 
        _bookingRepo.GetByIdAsync(id);
    public Task<List<Booking>> GetByUserId(int id) => 
        _bookingRepo.GetByUserIdAsync(id);
    public Task<List<Booking>> GetActiveBooking() => 
        _bookingRepo.GetActiveBookingsAsync();
    public Task<List<Booking>> GetByEventId(int eventId) => 
        _bookingRepo.GetByEventIdAsync(eventId);
    public Task<Booking?> GetByUserAndTickets(int userId, int ticketId) => 
        _bookingRepo.GetByUserAndTicketAsync(userId, ticketId);
}