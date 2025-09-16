using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponseDto>> GetById(int id)
        {
            var ticket = await _ticketService.GetById(id);
            return Ok(new TicketResponseDto(ticket.Id, ticket.Type, ticket.Price, ticket.EventId));
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<List<TicketDto>>> GetAll()
        {
            
            try
            {
                var tickets = await _ticketService.GetAllAsync();
                return Ok(tickets);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("byEvent/{eventId}")]
        
        public async Task<ActionResult<List<TicketDto>>> GetByEventIdAsync(int eventId)
        {
            try
            {
                var result = await _ticketService.GetByEventId(eventId);
                return Ok(result);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TicketResponseDto>> Create([FromBody] CreateTicketDto dto)
        {
            var ticket = new Ticket()
            {
                Type = dto.Type,
                Price = dto.Price,
                EventId = dto.EventId
            };
            try
            {
                var result = await _ticketService.CreateAsync(ticket);
                var response = new TicketResponseDto(result.Id, result.Type, result.Price, result.EventId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(int id)
        {
            await _ticketService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        {
            var ticket = new Ticket()
            {
                Id = id,
                Type = dto.Type,
                Price = dto.Price,
                EventId = dto.EventId
            };
            try
            {
                await _ticketService.UpdateAsync(ticket);
                ticket = await _ticketService.GetById(id);
                return Ok(new TicketResponseDto(ticket.Id,
                    ticket.Type, ticket.Price, ticket.EventId));
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
            
        }
    }
}