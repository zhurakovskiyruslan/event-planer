using EventPlanner.Application.Abstractions.Repositories;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using EventPlanner.Data.Enums;
using FluentValidation;
using ValidationException = EventPlanner.Application.Common.Exceptions.ValidationException;

namespace EventPlanner.Application.Services;

public class TicketService(
    ITicketRepository ticketRepository,
    IEventRepository eventRepository,
    IValidator<Ticket> ticketValidator)
    : ITicketService
{

    public async Task<Ticket> GetById(int ticketId)
    {
        var ticket = await ticketRepository.GetByIdAsync(ticketId);
        return ticket ?? throw new NotFoundException($"ticket with id {ticketId} not found");
    }

    public async Task<List<TicketDto>> GetAllAsync()
    {
        var tickets = await ticketRepository.GetAllAsync();
        return tickets.Select(MapToDto).ToList();
    }

    public async Task<List<TicketDto>> GetByEventId(int eventId)
    {
        var tickets = await ticketRepository.GetByEventIdAsync(eventId);
        return tickets.Select(MapToDto).ToList();
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        await ticketValidator.ValidateAndThrowAsync(ticket);
        var eventExist = await eventRepository.ExistsAsync(ticket.EventId);
        if (!eventExist)
            throw new NotFoundException("Event not found");
        try
        {
            var duplicates = await GetByEventId(ticket.EventId);
            if (duplicates.Any(duplicate => duplicate.TicketType == ticket.Type))
            {
                throw new ConflictException("Ticket already exists");
            }
        }
        catch (NotFoundException ex)
        {
            await ticketRepository.AddAsync(ticket);
            return ticket;
        }
        await ticketRepository.AddAsync(ticket);
        return ticket;
        
}

    public async Task UpdateAsync(Ticket ticket)
    {
       
        var ticketExist = await ticketRepository.ExistsAsync(ticket.Id);
        if (!ticketExist)
            throw new NotFoundException($"Ticket with id {ticket.Id} not found");
        await ticketValidator.ValidateAndThrowAsync(ticket);
        var eventExist = await eventRepository.ExistsAsync(ticket.EventId);
        if (!eventExist)
            throw new NotFoundException($"Event with id {ticket.EventId} not found");
        try
        {
            var duplicates = await GetByEventId(ticket.EventId);
            if (duplicates.Any(duplicate => duplicate.TicketType == ticket.Type))
            {
                throw new ConflictException("Ticket already exists");
            }
        }
        catch (NotFoundException ex)
        {
            await ticketRepository.UpdateAsync(ticket);
        }
        
        await ticketRepository.UpdateAsync(ticket);
        
    }
    
    public async Task DeleteAsync(int ticketId)
    {
        var ticketExist = await ticketRepository.ExistsAsync(ticketId);
        if (!ticketExist)
            throw new NotFoundException($"ticket with id {ticketId} not found");
        await ticketRepository.DeleteAsync(ticketId);
    }

    public static TicketDto MapToDto(Ticket t)
    {
        return new TicketDto(
            t.Id,
            t.Type,
            t.Price,
            t.EventId,
            t.Event.Title,
            t.Event.Description,
            t.Event.StartAtUtc,
            t.Event.Location.Name,
            t.Event.Location.Address
        );
    }
    
}