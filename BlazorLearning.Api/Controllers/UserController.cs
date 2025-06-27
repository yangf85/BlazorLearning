using BlazorLearning.Api.Models;
using BlazorLearning.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLearning.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userRepository.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound($"用户Id{id}不存在");
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> PostUser(User user)
    {
        try
        {
            var createdUser = await _userRepository.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            return BadRequest($"创建用户失败: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest("用户ID不匹配");
        }

        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound($"用户Id{id}不存在");
        }

        try
        {
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return BadRequest($"更新用户失败: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound($"用户Id{id}不存在");
        }
        try
        {
            var isDeleted = await _userRepository.DeleteUserAsync(id);
            if (isDeleted)
            {
                return NoContent();
            }
            else
            {
                return BadRequest("删除用户失败");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"删除用户失败: {ex.Message}");
        }
    }
}