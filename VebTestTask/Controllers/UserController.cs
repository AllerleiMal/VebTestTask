using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
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
    /// Gets Users after applying filtering, ordering and pagination 
    /// </summary>
    /// <returns>Response with List of Users of one page</returns>
    /// <response code="200">All query parameters are correct, returns requested Users</response>
    /// <response code="400">Incorrect query parameters</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
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
    /// Gets concrete user by ID
    /// </summary>
    /// <param name="id">Unique user ID</param>
    /// <returns>User with entered ID and attached roles</returns>
    /// <response code="200">User with such ID found successfully</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="403">No permissions for this action for your role</response>
    /// <response code="404">User with such ID found successfully</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        var targetUser = await _userRepository.GetUserByIdAsync(id);
        if (targetUser == null)
        {
            return NotFound(
                new Response<User>
                {
                    Message = $"No user with such ID {id}",
                    Succeeded = false
                });
        }

        return Ok(new Response<User>
        {
           Data = targetUser
        });
    }


    /// <summary>
    /// Deletes a specified user
    /// </summary>
    /// <param name="id">Unique user id</param>
    /// <returns></returns>
    /// <response code="204">Specified user deleted successfully</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="403">No permissions for this action for your role</response>
    /// <response code="404">Specified user is not found</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteUserByIdAsync(int id)
    {
        var targetUser = await _userRepository.DeleteUserAsync(id);

        if (targetUser is not null)
        {
            return NoContent();
        }
        
        return NotFound(
            new Response<User>
            {
                Message = $"No user with such ID {id}",
                Succeeded = false
            });
    }

    /// <summary>
    /// Creates User with form
    /// </summary>
    /// <param name="user">User entity from request body</param>
    /// <returns>Created User</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Provided User is null or there is a validation error</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="403">No permissions for this action for your role</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    [Authorize(Roles = "Admin,SuperAdmin,Support")]
    public async Task<IActionResult> CreateUserAsync([FromBody] User? user)
    {
        if (user is null)
        {
            return BadRequest(
                new Response<User>
                {
                    Message = "No user entity in request's body",
                    Succeeded = false
                });
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

    /// <summary>
    /// Updates specified User
    /// </summary>
    /// <param name="userToUpdate">Updated user from request body</param>
    /// <returns></returns>
    /// <response code="204">Updates applied successfully</response>
    /// <response code="400">Provided User is null or there is a validation error</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="403">No permissions for this action for your role</response>
    /// <response code="404">No user with such ID</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,SuperAdmin,Support")]
    public async Task<ActionResult> UpdateUserAsync([FromBody] User? userToUpdate)
    {
        if (userToUpdate is null)
        {
            return BadRequest(
                new Response<User>
                {
                    Message = "No user entity in request's body",
                    Succeeded = false
                });
        }

        var targetUser = await _userRepository.GetUserByIdAsync(userToUpdate.Id);

        if (targetUser is null)
        {
            return NotFound(
                new Response<User>
                {
                    Message = $"User with id {userToUpdate.Id} not found.",
                    Succeeded = false
                });
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

        return NoContent();
    }

    /// <summary>
    /// Gets all defined roles
    /// </summary>
    /// <returns>List of defined Roles</returns>
    /// <response code="200">Returns all defined Roles</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> GetAllRolesAsync()
    {
        var result = await _roleRepository.GetRolesAsync();
        return Ok(new Response<List<Role>>
        {
            Data = result.ToList()
        });
    }

    /// <summary>
    /// Adds Role with specified ID to the User with specified ID
    /// </summary>
    /// <param name="userId">Target User ID</param>
    /// <param name="newRoleId">Target Role ID</param>
    /// <returns>Changed User</returns>
    /// <response code="200">Role added to the target User</response>
    /// <response code="304">Target User already has specified Role, no data modification</response>
    /// <response code="401">User is not authorised</response>
    /// <response code="403">No permissions for this action for your role</response>
    /// <response code="404">Target User or Role is not found</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpGet("add_role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,SuperAdmin,Support")]
    public async Task<IActionResult> AddNewRole(int userId, int newRoleId)
    {
        var targetUser = await _userRepository.GetUserByIdAsync(userId);
        if (targetUser is null)
        {
            return NotFound(
                new Response<User>
                {
                    Message = $"No user with such id {userId}.",
                    Succeeded = false
                });
        }

        var targetRole = await _roleRepository.GetRoleByIdAsync(newRoleId);

        if (targetRole is null)
        {
            return NotFound(
                new Response<User>
                {
                    Message = $"No role with such id {newRoleId}.",
                    Succeeded = false
                });
        }

        var changedUser = await _userRepository.AddNewRoleForUserAsync(targetUser, targetRole);

        return changedUser is null
            ? StatusCode(304)
            : Ok(new Response<User>
            {
                Data = changedUser
            });
    }
}