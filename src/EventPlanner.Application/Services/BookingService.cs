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

    /// <summary>Создать бронь: проверяем пользователя, билет и запрет на дубль.</summary>
    public async Task<Booking> CreateAsync(int userId, int ticketId)
    {
        var user = await _userRepo.GetByIdAsync(userId)
                   ?? throw new InvalidOperationException("Пользователь не найден");

        var ticket = await _ticketRepo.GetByIdAsync(ticketId)
                     ?? throw new InvalidOperationException("Билет не найден");

        var duplicate = await _bookingRepo.GetByUserAndTicketAsync(userId, ticketId);
        if (duplicate != null)
            throw new InvalidOperationException("У этого пользователя уже есть бронь на этот билет");

        var booking = new Booking(user.Id, ticket.Id);
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

    public Task<Booking?> GetByIdAsync(int id) => _bookingRepo.GetByIdAsync(id);
    public Task<List<Booking>> GetByUserAsync(int userId) => _bookingRepo.GetByUserIdAsync(userId);
}