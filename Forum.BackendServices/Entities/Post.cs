using System.ComponentModel.DataAnnotations;
using Forum.BackendServices.Entities.Abstractions;

namespace Forum.BackendServices.Entities;

public class Post : IEntityBase
{
	public Guid Id { get; set; }

	[Required] 
	public string Title { get; set; } = null!;

	[Required] 
	public string Text { get; set; } = null!;

	public int PostUpPoints { get; set; } = 1;
	
	public List<PostAttachment> Attachments { get; set; } = new();

	public DateTime DateOfCreate { get; set; } = DateTime.UtcNow;
	
	public Guid OwnerId { get; set; }

	public User Owner { get; set; } = null!;

	public List<Comment> Comments { get; set; } = new();
}