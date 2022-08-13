using Forum.Api.Constants;
using Forum.Api.DTOs;
using Forum.Api.Interfaces;
using Forum.Contracts.Comment;
using Forum.Contracts.StatusCode;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
	private readonly IGuidService _guidService;
	private readonly ICommentService _commentService;
	private readonly IFileService _fileService;

	public CommentsController(IGuidService guidService, ICommentService commentService, IFileService fileService)
	{
		_guidService = guidService;
		_commentService = commentService;
		_fileService = fileService;
	}

	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(CommentDto), 200)]
	[HttpGet("{commentId}")]
	public async Task<IActionResult> GetComment(string commentId)
	{
		if (!_guidService.TryStringConvertToGuid(commentId, out var commentGuid))
			return BadRequest(new ResponseStatusCode4XX("commentId не являeтся Guid"));

		var comment = await _commentService.GetCommentAsync(commentGuid);

		if (comment == null) return NotFound(new ResponseStatusCode4XX("Комментарий не найден")); 
		
		return Ok(new CommentDto(comment));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(List<CommentDto>), 200)]
	[HttpGet("post/{postId}")]
	public async Task<IActionResult> GetComments(string postId)
	{
		if (!_guidService.TryStringConvertToGuid(postId, out var postGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var comments = await _commentService.GetCommentsAsync(postGuid);

		return Ok(comments.Select(c => new CommentDto(c)).ToList());
	}

	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(List<CommentDto>), 200)]
	[HttpGet("comment/{commentId}")]
	public async Task<IActionResult> GetCommentsByComment(string commentId)
	{
		if (!_guidService.TryStringConvertToGuid(commentId, out var commentGuid))
			return BadRequest(new ResponseStatusCode4XX("postId не является Guid"));

		var comments = await _commentService.GetCommentsByCommentAsync(commentGuid);

		return Ok(comments.Select(c => new CommentDto(c)).ToList());
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(CommentDto), 201)]
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> CreateComment([FromForm] CommentRequest commentRequest)
	{
		var user = HttpContext.User;
		var claimId = user.FindFirst(ClaimConstants.ID);
		if (claimId == null) return Unauthorized(new ResponseStatusCode4XX("Unauthorized"));

		if (commentRequest.Files != null)
		{
			if (commentRequest.Files.Count > 4) return BadRequest(new ResponseStatusCode4XX("Максимальное кол-во файлов: 4"));
			if (!_fileService.IsFileContentType(commentRequest.Files, MediaTypeNamesConstants.ImageJpeg, MediaTypeNamesConstants.ImagePng)) 
				return BadRequest(new ResponseStatusCode4XX($"Файл не соответствует поддерживаемым расширениям: {MediaTypeNamesConstants.ImageJpeg}, {MediaTypeNamesConstants.ImagePng}"));
		}

		if (!_guidService.TryStringConvertToGuid(claimId.Value, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));
		
		var createdComment = await _commentService.CreateCommentAsync(commentRequest, userGuid, commentRequest.Files);
			
		return Created($"/api/comments/{createdComment.Post.Id}/{createdComment.Id}", new CommentDto(createdComment));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(CommentDto), 201)]
	[Authorize]
	[HttpPut]
	public async Task<IActionResult> UpdateComment([FromForm] CommentUpdate commentUpdate)
	{
		var user = HttpContext.User;
		var claimId = user.FindFirst(ClaimConstants.ID);
		if (claimId == null) return Unauthorized(new ResponseStatusCode4XX("Unauthorized"));

		if (!_guidService.TryStringConvertToGuid(claimId.Value, out var userGuid))
			return BadRequest(new ResponseStatusCode4XX("userId не является Guid"));
		
		if (commentUpdate.Files != null)
		{
			if (commentUpdate.Files.Count > 4) return BadRequest(new ResponseStatusCode4XX("Максимальное кол-во файлов: 4"));
			if (!_fileService.IsFileContentType(commentUpdate.Files, MediaTypeNamesConstants.ImageJpeg, MediaTypeNamesConstants.ImagePng)) 
				return BadRequest(new ResponseStatusCode4XX($"Файл не соответствует поддерживаемым расширениям: {MediaTypeNamesConstants.ImageJpeg}, {MediaTypeNamesConstants.ImagePng}"));
		}

		var updatedComment = await _commentService.UpdateCommentAsync(commentUpdate, userGuid, commentUpdate.Files);

		return Ok(new CommentDto(updatedComment));
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(CommentDto), 201)]
	[Authorize]
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteComment(string id)
	{
		var user = HttpContext.User;
		var claimId = user.FindFirst(ClaimConstants.ID);
		if (claimId == null) return Unauthorized(new ResponseStatusCode4XX("Unauthorized"));

		if (!_guidService.TryStringConvertToGuid(claimId.Value, out var userGuid) ||
		    !_guidService.TryStringConvertToGuid(id, out var commentGuid))
			return BadRequest(new ResponseStatusCode4XX("userId или commentId не являются Guid"));
		
		var findComment = await _commentService.ValidateOwnerCommentAsync(userGuid, commentGuid);
		if (findComment == null) return new ObjectResult(new ResponseStatusCode4XX("Нельзя удалить чужой комментарий"))
		{
			StatusCode = 403
		};

		var deletedComment = await _commentService.DeleteCommentAsync(findComment);
		
		return Ok(new CommentDto(deletedComment));
	}
}
