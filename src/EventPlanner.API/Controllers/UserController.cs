using EventPlanner.API.Contracts;
using EventPlanner.Application.Abstractions.Services;
using EventPlanner.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EventPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService; 
        }
      

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetById(id);
            return Ok(new UserResponseDto(user.Id, user.Name, user.Email));
        }

        // GET: api/<UserController>
        [HttpGet("by-email/{email}", Name = "GetUserByEmail")]
        public async Task<ActionResult<UserResponseDto>> GetByEmailAsync(string email)
        {
            var user = await _userService.GetByEmail(email);
            return Ok(new UserResponseDto(user.Id, user.Name, user.Email));
        }
        
        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
        {
            var user = new User()
            {
                Name = dto.Name,
                Email = dto.Email
            };
            var result = await _userService.CreateAsync(user);
            var response = new UserResponseDto(result.Id, result.Name, result.Email);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
        }
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var user = new User()
            {
                Id = id,
                Name = dto.Name,
                Email = dto.Email
            };
            await _userService.UpdateAsync(user);
            var response = new UserResponseDto(user.Id, user.Name, user.Email);
            return Ok(response);
        }
       

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
