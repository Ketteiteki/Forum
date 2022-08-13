using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Forum.Contracts.Post;

public class PostRequest
{
	[Required]
	public string Title { get; set; } = null!;
	
	[Required]
	public string Text { get; set; } = null!;
	
	public IFormFileCollection? Files { get; set; }
}