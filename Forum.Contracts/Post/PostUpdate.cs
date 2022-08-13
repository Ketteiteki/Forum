using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Forum.Contracts.Post;

public class PostUpdate
{
	[Required]
	public Guid Id { get; set; } = new Guid();
	
	[Required]
	public string Title { get; set; } = null!;
	
	[Required]
	public string Text { get; set; } = null!;
	
	public IFormFileCollection? Files { get; set; }
}