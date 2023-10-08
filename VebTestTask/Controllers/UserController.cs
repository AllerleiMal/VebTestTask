using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VebTestTask.Data;
using VebTestTask.Data.Repositories;
using VebTestTask.Filter;
using VebTestTask.Models;
using VebTestTask.Wrapper;

namespace VebTestTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserContext _context;
    private readonly IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger, UserContext context, IUserRepository repository)
    {
        _logger = logger;
        _context = context;
        _userRepository = repository;
    }

    /// <summary>
    /// Get all Users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationFilter filter)
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
    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(long id)
    {
        var targetUser = await _context.Users.Where(user => user.Id == id).Include(user => user.Roles).FirstOrDefaultAsync();
        if (targetUser == null)
        {
            return NotFound();
        }

        return Ok(targetUser);
    }

    
    /// <summary>
    /// Deletes a specified user
    /// </summary>
    /// <param name="id">Unique user id</param>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserById(long id)
    {
        var targetUser = await _context.Users.FindAsync(id);
        if (targetUser is null)
        {
            return NotFound();
        }

        _context.Users.Remove(targetUser);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}