using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class LocationController : ControllerBase
{
    private readonly ILocationService _service;

    public LocationController(ILocationService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<LocationResponseDto>> Create([FromBody] CreateLocationDto dto)
    {
        var location = new Location()
        {
            Name = dto.Name,
            Address = dto.Address,
            Capacity = dto.Capacity
        };
        var result = await _service.CreateAsync(location);
        var response = new LocationResponseDto(location.Id, location.Name, location.Address, location.Capacity);
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, response);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<LocationResponseDto>> GetById(int id)
    {
        var loc = await _service.GetByIdAsync(id);
        return Ok(new LocationResponseDto(loc.Id, loc.Name, loc.Address, loc.Capacity));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateLocationDto dto)
    {
        var location = new Location()
        {
            Id = id,
            Name = dto.Name,
            Address = dto.Address,
            Capacity = dto.Capacity
        };
        await _service.UpdateAsync(location);
        var response = new LocationResponseDto(location.Id, location.Name, 
            location.Address, location.Capacity);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<List<LocationResponseDto>>> GetAll()
    {
        var locations = await _service.GetAllAsync();
        return Ok(locations.Select(l => new LocationResponseDto(l.Id, l.Name, l.Address, l.Capacity)));
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> Delete(int id)
    {
       await _service.DeleteAsync(id);
       return NoContent();  
    }
}