using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Forum.Contracts.Comment;

public class CommentRequest
{
	[StringLength(10000, MinimumLength = 1, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Text { get; set; } = null!;
	
	[Required]
	public bool IsSage { get; set; } = false;
	
	public Guid? ResponseToCommentId { get; set; }
	
	[Required]
	public Guid PostId { get; set; }
	
	public IFormFileCollection? Files { get; set; }
}