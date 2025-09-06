using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Abstractions.Services;

public interface ITicketService
{
    Task<Ticket> GetById(int ticketId);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task DeleteAsync(int id);
}