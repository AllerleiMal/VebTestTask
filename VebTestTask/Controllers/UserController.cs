using Microsoft.AspNetCore.Mvc;
using VebTestTask.Models;

namespace VebTestTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(new List<User>
        {
            new()
            {
                Id = 1,
                Name = "Nick",
                Age = 29,
                Email = "alalla@gmail.com"
            }
        });
    }
}