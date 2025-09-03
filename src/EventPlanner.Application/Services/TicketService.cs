using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;

namespace EventPlanner.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public Task<Ticket> GetById(int ticketId) => 
        _ticketRepository.GetByIdAsync(ticketId);
    
    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        await _ticketRepository.AddAsync(ticket);
        return ticket;
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        if (ticket is null) throw new ArgumentNullException(nameof(ticket));
        await _ticketRepository.UpdateAsync(ticket);
    }
    
    public async Task DeleteAsync(int ticketId)=>
        await _ticketRepository.DeleteAsync(ticketId);
}