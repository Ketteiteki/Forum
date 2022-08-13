using System.ComponentModel.DataAnnotations;
using Forum.BackendServices.Entities.Abstractions;

namespace Forum.BackendServices.Entities;

public class Comment : IEntityBase
{
	public Guid Id { get; set; }

	[StringLength(10000, MinimumLength = 1, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Text { get; set; }
	
	public bool IsOp { get; set; } = false;

	public bool IsSage { get; set; } = false;
	
	public Guid? ResponseToCommentId { get; set; }
	
	public Comment? ResponseToComment { get; set; } = null;
	
	public List<Comment> Comments = new();
	
	public List<CommentAttachment> Attachments { get; set; } = new();

	public DateTime DateOfCreate { get; set; } = DateTime.UtcNow;

	public Guid OwnerId { get; set; }

	public User Owner { get; set; } = null!;
	
	public Guid PostId { get; set; }

	public Post Post { get; set; } = null!;
}