using Microsoft.AspNetCore.Mvc;
using WebApiHttpExample.Models;
using WebApiHttpExample.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiHttpExample.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsersRepository _usersRepository;

    public UsersController(IUsersRepository userRepository)
    {
        _usersRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    [HttpGet("{id}", Name = "GetUserById")] // Added Name for Url.Link to reference
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _usersRepository.GetUserByIdFromDatabaseAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdUser = await _usersRepository.CreateUserInDatabaseAsync(user);
        var uri = Url.Link("GetUserById", new { id = createdUser.UserId });
        if (uri == null)
        {
            // Fallback or error handling if route cannot be generated
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not generate location URI.");
        }

        return Created(uri, createdUser);
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateOrCreateUser([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (user.UserId > 0)
        {
            var updated = await _usersRepository.UpdateUserInDatabaseAsync(user);
            if (updated)
            {
                return Accepted();
            }

            return NotFound();
        }

        var createdUser = await _usersRepository.CreateUserInDatabaseAsync(user);
        var uri = Url.Link("GetUserById", new { id = createdUser.UserId });

        if (uri == null)
        {
            // Fallback or error handling if route cannot be generated
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not generate location URI.");
        }

        return Created(uri, createdUser);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PartialUpdateUser(int id, [FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = await _usersRepository.PartiallyUpdateUserInDatabaseAsync(id, user);
        if (updated)
        {
            return Accepted();
        }

        return NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserById(int id)
    {
        var deleted = await _usersRepository.DeleteUserByIdInDatabaseAsync(id);
        if (deleted)
        {
            return Accepted();
        }

        return NotFound();
    }
}
