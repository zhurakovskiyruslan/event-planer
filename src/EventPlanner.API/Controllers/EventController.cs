using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Application.ReadModels;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] CreateEventDto dto)
        {
            var entity = new Event()
            {
                Title = dto.Title,
                Description = dto.Description,
                StartAtUtc = dto.StartAtUtc,
                Capacity =  dto.Capacity,
                LocationId = dto.LocationId
            };
            var result = await _eventService.CreateAsync(entity);
            var response = new EventResponseDto(entity.Id, entity.Title, entity.Description, entity.StartAtUtc, entity.Capacity, entity.LocationId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetById(int id) => await _eventService.GetById(id);

        [HttpGet]
        public async Task<ActionResult<List<EventDto>>> GetAll() => await _eventService.GetAllAsync();
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _eventService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            var entity = new Event()
            {
                Id = id,
                Title = dto.Title,
                Description = dto.Description,
                StartAtUtc = dto.StartAtUtc,
                Capacity =  dto.Capacity,
                LocationId = dto.LocationId
            }; 
            await _eventService.UpdateAsync(entity);
            var response = new EventResponseDto(entity.Id, entity.Title, 
                entity.Description, entity.StartAtUtc, entity.Capacity, entity.LocationId);
            return Ok(response);
        }
    }
}

