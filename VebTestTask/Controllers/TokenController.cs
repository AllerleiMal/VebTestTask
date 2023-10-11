using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VebTestTask.Data.Repositories;
using VebTestTask.Models;
using VebTestTask.Wrapper;

namespace VebTestTask.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[AllowAnonymous]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TokenController> _logger;
    private readonly IValidator<LoginCredentials> _loginValidator;

    public TokenController(IConfiguration config, IUserRepository userRepository, ILogger<TokenController> logger,
        IValidator<LoginCredentials> loginValidator)
    {
        _logger = logger;
        _configuration = config;
        _userRepository = userRepository;
        _loginValidator = loginValidator;
    }

    /// <summary>
    /// Returns JWT security token of user with specified name and email address
    /// </summary>
    /// <param name="loginCredentials">Login credentials of User</param>
    /// <returns>JWT token</returns>
    /// <response code="200">All query parameters are correct, returns token</response>
    /// <response code="400">No entity in request body or validation failed</response>
    /// <response code="404">User with such credentials is not found</response>
    /// <response code="500">Unhandled exception during request processing</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] LoginCredentials? loginCredentials)
    {
        if (loginCredentials is null)
        {
            return BadRequest(new Response<string>
            {
                Succeeded = false,
                Message = "No login credentials in request's body"
            });
        }

        var result = await _loginValidator.ValidateAsync(loginCredentials);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return BadRequest(new Response<LoginCredentials>
            {
                Data = loginCredentials,
                Errors = result.Errors.Select(x => x.ToString()).ToArray(),
                Message = "Credentials validation is not passed",
                Succeeded = false
            });
        }

        var targetUser =
            await _userRepository.GetUserByNameAndEmailAsync(loginCredentials.Name, loginCredentials.Email);

        if (targetUser == null)
        {
            return NotFound(new Response<string>
            {
                Succeeded = false,
                Message =
                    $"User with name {loginCredentials.Name} and email address {loginCredentials.Email} is not found"
            });
        }

        var token = GetJwtTokenForUser(targetUser);
        
        _logger.LogInformation(
            $"User with email {targetUser.Email} authorized successfully, provided role is {targetUser.Roles.MaxBy(x => x.Id).Name}");
        
        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }

    private Claim[] GetClaimsForUser(User targetUser)
    {
        return new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new Claim("UserId", targetUser.Id.ToString()),
            new Claim("Name", targetUser.Name),
            new Claim("Email", targetUser.Email),
            new Claim("Age", targetUser.Age.ToString()),
            new Claim(ClaimTypes.Role, targetUser.Roles.MaxBy(x => x.Id).Name)
        };
    }

    private JwtSecurityToken GetJwtTokenForUser(User targetUser)
    {
        var claims = GetClaimsForUser(targetUser);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(3),
            signingCredentials: signIn);

        return token;
    }
}