using Forum.Api.DTOs;
using Forum.Api.Interfaces;
using Forum.BackendServices.Entities.Enums;
using Forum.Contracts.StatusCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers;

[Authorize(Roles = $"{nameof(Roles.Moderator)}")]
[Route("api/[controller]")]
[ApiController]
public class ModeratorController : ControllerBase
{
	private readonly IGuidService _guidService;
	private readonly IPostService _postService;
	private readonly ICommentService _commentService;
	private readonly IUserService _userService;

	public ModeratorController(IGuidService guidService, IPostService postService, ICommentService commentService,
		IUserService userService)
	{
		_guidService = guidService;
		_postService = postService;
		_commentService = commentService;
		_userService = userService;
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(UserDto), 200)]
	[Authorize(Roles = $"{nameof(Roles.Moderator)}")]
	[HttpPost("banUser/{userId}")]
	public async Task<IActionResult> BanUser(string userId, [FromQuery] int s = 0)
	{
		if (!_guidService.TryStringConvertToGuid(userId, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));
		
		if (s <= 0) return BadRequest(new ResponseStatusCode4XX("Время бана не может быть меньше и равняться нулю"));

		var dateBan = DateTime.UtcNow.AddSeconds(s);

		var user = await _userService.GetUserAsync(userGuid);
		if (user == null) return NotFound(new ResponseStatusCode4XX("Пользователь не найден"));
		
		if (user.Role == Roles.Admin.ToString() || user.Role == Roles.Moderator.ToString())
			return new ObjectResult(new ResponseStatusCode4XX("Нельзя забанить пользователей вашей роли или выше"))
			{
				StatusCode = 403
			};
		
		var bannedUser = await _userService.BanUserAsync(user, dateBan);

		return Ok(new UserDto(bannedUser));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(PostDto), 200)]
	[Authorize(Roles = $"{nameof(Roles.Moderator)}")]
	[HttpDelete("deletePost/{id}")]
	public async Task<IActionResult> DeletePost(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var postGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var post = await _postService.GetPostAsync(postGuid);
		if (post == null)
			return NotFound(new ResponseStatusCode4XX("Пост не найден"));

		if (post.Owner.Role == Roles.Admin.ToString()) 
			return new ObjectResult(new ResponseStatusCode4XX("Нельзя удалить пост пользователя, занимающий роль выше"))
			{
				StatusCode = 403
			};

		var deletedPost = await _postService.DeletePostAsync(post);
		
		return Ok(new PostDto(deletedPost));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(CommentDto), 200)]
	[Authorize(Roles = $"{nameof(Roles.Moderator)}")]
	[HttpDelete("deleteComment/{id}")]
	public async Task<IActionResult> DeleteComment(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var commentGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var comment = await _commentService.GetCommentAsync(commentGuid);
		if (comment == null)
			return NotFound(new ResponseStatusCode4XX("Комментарий не найден"));
		
		if (comment.Owner.Role == Roles.Admin.ToString())
			return new ObjectResult(new ResponseStatusCode4XX("Нельзя удалить комментарий пользователя, занимающий роль выше"))
			{
				StatusCode = 403
			};
		
		var deletedComment = await _commentService.DeleteCommentAsync(comment);
		
		return Ok(new CommentDto(deletedComment));
	}
}