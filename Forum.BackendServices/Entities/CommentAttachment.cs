using System.ComponentModel.DataAnnotations;
using Forum.BackendServices.Entities.Abstractions;

namespace Forum.BackendServices.Entities;

public class CommentAttachment : IEntityBase
{
	public Guid Id { get; set; }

	[Required]
	public string FileName { get; set; } = Guid.NewGuid().ToString();
	
	[Required]
	public double FileSize { get; set; }

	public Guid? CommentId { get; set; } = null;

	public Comment? Comment { get; set; } = new();
}