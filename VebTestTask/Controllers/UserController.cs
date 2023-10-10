using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VebTestTask.Data;
using VebTestTask.Data.Repositories;
using VebTestTask.Filter;
using VebTestTask.Models;
using VebTestTask.Wrapper;

namespace VebTestTask.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    /// <summary>
    /// Get all Users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersAsync([FromQuery] PaginationFilter filter)
    {

        var pagedUsersParams = await PaginatedUsersParams.GetParamsFromPaginationFilter(filter);
        if (pagedUsersParams is null)
        {
            return BadRequest("Error in parameters");
        }
        
        var (pagedData, totalRecords) = await _userRepository.GetPaginatedUsersAsync(pagedUsersParams);
        
        var response = new PagedResponse<List<User>>
        {
            Data = pagedData.ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Get concrete user by ID
    /// </summary>
    /// <param name="id">Unique user ID</param>
    /// <returns>User with entered ID and attached roles</returns>
    /// <response code="200">User with such ID found successfully</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        var targetUser = await _userRepository.GetUserByIdAsync(id);
        if (targetUser == null)
        {
            return NotFound("No user with such ID");
        }

        return Ok(targetUser);
    }

    
    /// <summary>
    /// Deletes a specified user
    /// </summary>
    /// <param name="id">Unique user id</param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserByIdAsync(int id)
    {
        var targetUser = await _userRepository.DeleteUserAsync(id);
        if (targetUser is null)
        {
            return BadRequest("No user with such ID");
        }
        
        return NoContent();
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUserAsync([FromBody]User? user)
    {
        try
        {
            if (user is null)
            {
                return BadRequest("No user entity in request's body");
            }
    
            var existedUser = await _userRepository.GetUserByEmailAsync(user.Email);
            
            if (existedUser != null)
            {
                ModelState.AddModelError("email", "User email is already in use");
                return BadRequest(ModelState);
            }
            
            var addedUser = await _userRepository.InsertUserAsync(user);
    
            return CreatedAtAction(nameof(GetUserByIdAsync), new { id = addedUser.Id }, addedUser);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error while creating user");
        }
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> UpdateProductAsync([FromBody] User userToUpdate)
    {
        var targetUser = await _userRepository.GetUserByIdAsync(userToUpdate.Id);

        if (targetUser is null)
        {
            return NotFound($"User with id {userToUpdate.Id} not found.");
        }

        await _userRepository.UpdateUserAsync(userToUpdate);

        return CreatedAtAction(nameof(GetUserByIdAsync), new { id = userToUpdate.Id }, null);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        return Ok(await _roleRepository.GetRolesAsync());
    }

    [HttpGet("add_role")]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNewRole(int userId, int newRoleId)
    {
        var targetUser = await _userRepository.GetUserByIdAsync(userId);
        if (targetUser is null)
        {
            return NotFound($"No user with such id {userId}");
        }

        var targetRole = await _roleRepository.GetRoleByIdAsync(newRoleId);

        if (targetRole is null)
        {
            return NotFound($"No role with such id {newRoleId}");
        }

        var changedUser = await _userRepository.AddNewRoleForUser(targetUser, targetRole);

        return changedUser is null ? StatusCode(304, targetUser) : Ok(changedUser);
    }
}