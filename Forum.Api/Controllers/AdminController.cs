using Forum.Api.DTOs;
using Forum.Api.Interfaces;
using Forum.BackendServices.Entities.Enums;
using Forum.Contracts.StatusCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers;

[Authorize(Roles = $"{nameof(Roles.Admin)}")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
	private readonly IGuidService _guidService;
	private readonly IAdminService _adminService;
	private readonly IPostService _postService;
	private readonly ICommentService _commentService;
	private readonly IUserService _userService;
	private readonly IEnumService _enumService;

	public AdminController(IGuidService guidService, IAdminService adminService, IPostService postService,
		ICommentService commentService, IUserService userService, IEnumService enumService)
	{
		_guidService = guidService;
		_adminService = adminService;
		_postService = postService;
		_commentService = commentService;
		_userService = userService;
		_enumService = enumService;
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpPost("banUser/{userId}")]
	public async Task<IActionResult> BanUser(string userId, [FromQuery] int s = 0)
	{
		if (!_guidService.TryStringConvertToGuid(userId, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));
		
		if (s <= 0) return BadRequest(new ResponseStatusCode4XX("Время бана не может быть меньше и равняться нулю"));
	
		var dateBan = DateTime.UtcNow.AddSeconds(s);
	
		var user = await _userService.GetUserAsync(userGuid);
		if (user == null) return NotFound(new ResponseStatusCode4XX("Пользователь не найден"));
		
		var bannedUser = await _userService.BanUserAsync(user, dateBan);
	
		return Ok(new UserDto(bannedUser));
	}

	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(200)]
	[HttpPost("setRole/{userId}")]
	public async Task<IActionResult> SetRole(string userId, [FromQuery] string role)
	{
		if (!_guidService.TryStringConvertToGuid(userId, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));

		var user = await _userService.GetUserAsync(userGuid);
		if (user == null) return NotFound(new ResponseStatusCode4XX("Пользователь не найден"));

		if (!_enumService.TryParseEnum<Roles>(role, out var resultRole))
			return BadRequest(new ResponseStatusCode4XX("Роли не существует"));
		if (resultRole == Roles.Admin)
			return BadRequest(new ResponseStatusCode4XX("Нельзя повысить пользователя до своей роли и выше"));

		await _adminService.SetRoleAsync(user, resultRole);
		
		return Ok();
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(PostDto), 200)]
	[HttpDelete("deletePost/{id}")]
	public async Task<IActionResult> DeletePost(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var postGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var findPost = await _postService.GetPostAsync(postGuid);
		if (findPost == null) return NotFound(new ResponseStatusCode4XX("Пост не найден"));
		
		var deletedPost = await _postService.DeletePostAsync(findPost);
		
		return Ok(new PostDto(deletedPost));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(CommentDto), 200)]
	[HttpDelete("deleteComment/{id}")]
	public async Task<IActionResult> DeleteComment(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var commentGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var findComment = await _commentService.GetCommentAsync(commentGuid);
		if (findComment == null) return NotFound(new ResponseStatusCode4XX("Комментарий не найден"));
		
		var deleteComment = await _commentService.DeleteCommentAsync(findComment);
		
		return Ok(new CommentDto(deleteComment));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[HttpDelete("deleteUser/{id}")]
	public async Task<IActionResult> DeleteUser(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));

		var findUser = await _userService.GetUserAsync(userGuid);
		if (findUser == null) return NotFound(new ResponseStatusCode4XX("Пользователь не найден"));
		
		var deletedUser = await _userService.DeleteUserAsync(findUser);
		
		return Ok(new UserDto(deletedUser));
	}
}