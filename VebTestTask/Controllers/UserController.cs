using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VebTestTask.Models;

namespace VebTestTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserContext _context;

    public UserController(ILogger<UserController> logger, UserContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Get all Users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.Include(u=>u.Roles).ToListAsync();
        return Ok(users);
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
        else
        {
            return Ok(targetUser);
        }
    }
    
}