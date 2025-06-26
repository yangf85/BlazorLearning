using BlazorLearning.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private static List<User> _users =
    [
        new User { Id = 1, UserName = "admin", Email = "admin@example.com", FullName = "系统管理员" },
        new User { Id = 2, UserName = "user1", Email = "user1@example.com", FullName = "用户一" },
        new User { Id = 3, UserName = "user2", Email = "user2@example.com", FullName = "用户二" }
    ];

    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        return Ok(_users);
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}