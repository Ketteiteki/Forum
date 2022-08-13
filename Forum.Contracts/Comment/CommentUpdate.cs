using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Forum.Contracts.Comment;

public class CommentUpdate
{
	[Required]
	public Guid Id { get; set; } = new Guid();

	[StringLength(10000, MinimumLength = 1, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Text { get; set; } = null!;
	
	public IFormFileCollection? Files { get; set; }
}