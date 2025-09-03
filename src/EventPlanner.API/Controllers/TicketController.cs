using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            if (ticket is null)
                return NotFound();
            return Ok(new TicketResponseDto(ticket.Id, ticket.Type, ticket.Price, ticket.EventId));
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponseDto>> Create([FromBody] CreateTicketDto dto)
        {
            var ticket = new Ticket()
            {
                Type = dto.Type,
                Price = dto.Price,
                EventId = dto.EventId
            };
            var result = await _ticketService.CreateAsync(ticket);

            var response = new TicketResponseDto(result.Id, result.Type, result.Price, result.EventId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _ticketService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        {
            var ticket = await _ticketService.GetById(id);
            if (ticket is null)
                return NotFound();
            
            ticket.Update(dto.Type, dto.Price, dto.EventId);
            ticket.Event = null;
            await _ticketService.UpdateAsync(ticket);
            
            return NoContent();
        }

    }
}