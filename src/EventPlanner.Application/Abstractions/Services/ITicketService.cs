using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Services;

public interface ITicketService
{
    Task<Ticket> GetById(int ticketId);
    Task<List<TicketDto>> GetByEventId(int eventId);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task DeleteAsync(int id);
}