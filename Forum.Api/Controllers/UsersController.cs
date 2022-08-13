using Forum.Api.DTOs;
using Forum.Api.Interfaces;
using Forum.Contracts.StatusCode;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly IGuidService _guidService;
	
	public UsersController(IUserService userService, IGuidService guidService)
	{
		_userService = userService;
		_guidService = guidService;
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpGet("{id}")]
	public async Task<IActionResult> GetUser(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));
		
		var user = await _userService.GetUserAsync(userGuid);

		if (user == null) return NotFound(new ResponseStatusCode4XX("Пользователь не найден"));

		return Ok(new UserDto(user));
	}
	
	[ProducesResponseType(typeof(List<UserDto>), 200)]
	[HttpGet]
	public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int count = 10)
	{
		var users = await _userService.GetUsersAsync(search, page, count);

		return Ok(users.Select(u => new UserDto(u)).ToList());
	}
}