using Forum.Api.Constants;
using Forum.Api.DTOs;
using Forum.Api.Interfaces;
using Forum.Contracts.Post;
using Forum.Contracts.StatusCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
	private readonly IGuidService _guidService;
	private readonly IPostService _postService;
	private readonly IFileService _fileService;
	
	public PostsController(IGuidService guidService, IPostService postService, IFileService fileService)
	{
		_guidService = guidService;
		_postService = postService;
		_fileService = fileService;
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(PostDto), 200)]
	[HttpGet("{id}")]
	public async Task<IActionResult> GetPost(string id)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var PostGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var post = await _postService.GetPostAsync(PostGuid);

		if (post == null) return NotFound(new ResponseStatusCode4XX("Пост не найден"));

		return Ok(new PostDto(post));
	}
	
	[ProducesResponseType(typeof(List<PostDto>), 200)]
	[HttpGet]
	public async Task<IActionResult> GetPosts([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int count = 10)
	{
		var posts = await _postService.GetPostsAsync(search, page, count);
		return Ok(posts.Select(p => new PostDto(p)).ToList());
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(List<PostDto>), 200)]
	[HttpGet("user/{id}")]
	public async Task<IActionResult> GetPostsByUser(string id, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int count = 10)
	{
		if (!_guidService.TryStringConvertToGuid(id, out var postGuid))
			return BadRequest("postId не является Guid");
			
		var posts = await _postService.GetPostsByUserAsync(postGuid, page, count);
		
		return Ok(posts.Select(p => new PostDto(p)).ToList());
	}

	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 401)]
	[ProducesResponseType(typeof(PostDto), 201)]
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> CreatePost([FromForm] PostRequest postRequest)
	{
		var user = HttpContext.User;
		var claimId = user.FindFirst(ClaimConstants.ID);
		if (claimId == null) return Unauthorized(new ResponseStatusCode4XX("Unauthorized"));

		if (postRequest.Files != null)
		{
			if (postRequest.Files.Count > 4) return BadRequest(new ResponseStatusCode4XX("Максимальное кол-во файлов: 4"));
			
			if (!_fileService.IsFileContentType(postRequest.Files, MediaTypeNamesConstants.ImageJpeg, MediaTypeNamesConstants.ImagePng))
				return BadRequest(new ResponseStatusCode4XX($"Файл не соответствует поддерживаемым расширениям: {MediaTypeNamesConstants.ImageJpeg}, {MediaTypeNamesConstants.ImagePng}"));
		}
		
		if (!_guidService.TryStringConvertToGuid(claimId.Value, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userID не является Guid"));
		
		var createdPost = await _postService.CreatePostAsync(postRequest, userGuid, postRequest.Files);
		
		return Created($"/api/posts/{createdPost.Id}", new PostDto(createdPost));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 401)]
	[ProducesResponseType(typeof(PostDto), 200)]
	[Authorize]
	[HttpPut]
	public async Task<IActionResult> UpdatePost([FromForm] PostUpdate postUpdate)
	{
		var user = HttpContext.User;
		var claimId = user.FindFirst(ClaimConstants.ID);
		if (claimId == null) return Unauthorized(new ResponseStatusCode4XX("Unauthorized"));

		if (!_guidService.TryStringConvertToGuid(claimId.Value, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userID не является Guid"));

		if (postUpdate.Files != null)
		{
			if (postUpdate.Files.Count > 4) return BadRequest(new ResponseStatusCode4XX("Максимальное кол-во файлов: 4"));
		
			if (!_fileService.IsFileContentType(postUpdate.Files, MediaTypeNamesConstants.ImageJpeg, MediaTypeNamesConstants.ImagePng))
				return BadRequest(new ResponseStatusCode4XX($"Файл не соответствует поддерживаемым расширениям: {MediaTypeNamesConstants.ImageJpeg}, {MediaTypeNamesConstants.ImagePng}"));
		}
		
		var updatedPost = await _postService.UpdatePostAsync(postUpdate, userGuid, postUpdate.Files);
		
		return Ok(new PostDto(updatedPost));
	}

	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 401)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(PostDto), 200)]
	[Authorize]
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeletePost(string id)
	{
		var user = HttpContext.User;
		var claimId = user.FindFirst(ClaimConstants.ID);
		if (claimId == null) return Unauthorized(new {Message = "Unauthorized"});
		
		if (!_guidService.TryStringConvertToGuid(claimId.Value, out var userGuid) ||
		    !_guidService.TryStringConvertToGuid(id, out var postGuid))
			return BadRequest(new {Message = "userID не является Guid"});

		var validatePost = await _postService.ValidateOwnerPostAsync(userGuid, postGuid);
		if (validatePost == null) return new ObjectResult(new {Message = "Нельзя удалять чужой пост"})
		{
			StatusCode = 403
		};
		
		var deletedPost = await _postService.DeletePostAsync(validatePost);
		
		return Ok(new PostDto(deletedPost));
	}
}