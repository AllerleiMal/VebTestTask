using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IValidator<User> _userValidator;

    public UserController(ILogger<UserController> logger, IUserRepository userRepository,
        IRoleRepository roleRepository, IValidator<User> userValidator)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userValidator = userValidator;
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
    public async Task<IActionResult> CreateUserAsync([FromBody] User? user)
    {
        if (user is null)
        {
            return BadRequest("No user entity in request's body");
        }

        user.Id = 0;
        var result = await _userValidator.ValidateAsync(user);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return BadRequest(new Response<User>
            {
                Data = user,
                Errors = result.Errors.Select(x => x.ToString()).ToArray(),
                Message = "User validation is not passed",
                Succeeded = false
            });
        }

        var addedUser = await _userRepository.InsertUserAsync(user);

        return CreatedAtAction(nameof(GetUserByIdAsync), new { id = addedUser.Id }, new Response<User?>(addedUser));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> UpdateUserAsync([FromBody] User userToUpdate)
    {
        var targetUser = await _userRepository.GetUserByIdAsync(userToUpdate.Id);

        if (targetUser is null)
        {
            return NotFound($"User with id {userToUpdate.Id} not found.");
        }

        var result = await _userValidator.ValidateAsync(userToUpdate);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return BadRequest(new Response<User>
            {
                Data = userToUpdate,
                Errors = result.Errors.Select(x => x.ToString()).ToArray(),
                Message = "User validation is not passed",
                Succeeded = false
            });
        }

        await _userRepository.UpdateUserAsync(userToUpdate);

        return CreatedAtAction(nameof(GetUserByIdAsync), new { id = userToUpdate.Id }, new Response<User?>());
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        var result = await _roleRepository.GetRolesAsync();
        return Ok(new Response<List<Role>>
        {
            Data = result.ToList()
        });
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

        return changedUser is null
            ? StatusCode(304)
            : Ok(new Response<User>
            {
                Data = changedUser
            });
    }
}