using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums;

namespace EventPlanner.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IEventRepository _eventRepository;

    public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository)
    {
        _ticketRepository = ticketRepository;
        _eventRepository = eventRepository;
    }

    public async Task<Ticket> GetById(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException($"ticket with id {ticketId} not found");
        return ticket;
    }
    
    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        if (ticket == null)
            throw new ValidationException("Ticket is required");
        if (ticket.Price <= 0)
            throw new ValidationException("Ticket price must be greater than zero");
        if (ticket.Type != TicketType.Standard &&
            ticket.Type != TicketType.VIP)
            throw new ValidationException("Ticket type must be VIP or Standard");
        var eventExist = await _eventRepository.ExistsAsync(ticket.EventId);
        if (!eventExist)
            throw new NotFoundException("Event not found");
        await _ticketRepository.AddAsync(ticket);
        return ticket;
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        if (ticket is null) throw new ArgumentNullException(nameof(ticket));
        var ticketExist = await _ticketRepository.ExistsAsync(ticket.Id);
        if (!ticketExist)
            throw new NotFoundException($"Ticket with id {ticket.Id} not found");
        if (ticket.Price <= 0)
            throw new ValidationException("Ticket price must be greater than zero");
        if (!Enum.IsDefined(typeof(TicketType), ticket.Type))
            throw new ValidationException("Invalid ticket type");
        var eventExist = await _eventRepository.ExistsAsync(ticket.EventId);
        if (!eventExist)
            throw new NotFoundException($"Event with id {ticket.EventId} not found");
        await _ticketRepository.UpdateAsync(ticket);
    }
    
    public async Task DeleteAsync(int ticketId)
    {
        var ticketExist = await _ticketRepository.ExistsAsync(ticketId);
        if (!ticketExist)
            throw new NotFoundException($"ticket with id {ticketId} not found");
        await _ticketRepository.DeleteAsync(ticketId);
    }
}