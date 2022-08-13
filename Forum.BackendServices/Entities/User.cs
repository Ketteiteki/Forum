using System.ComponentModel.DataAnnotations;
using Forum.BackendServices.Entities.Abstractions;
using Forum.BackendServices.Entities.Enums;
using Newtonsoft.Json;

namespace Forum.BackendServices.Entities;

public class User : IEntityBase
{
	public Guid Id { get; set; } = new Guid();
	
	[EmailAddress]
	[Required]
	public string Email { get; set; } = null!;
	
	[StringLength(20, MinimumLength = 4, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Login { get; set; } = null!;
	
	[Required]
	public string Password { get; set; } = null!;
	
	public string? AvatarLink { get; set; }
	
	public string Role { get; set; } = Roles.User.ToString();
	
	public bool IsBanned { get; set; } = false;
	
	public DateTime? BanExpires { get; set; } = null;

	public bool EmailConfirm { get; set; } = false;

	public Guid ActivationCode { get; set; } = Guid.NewGuid();

	public RefreshToken? RefreshToken { get; set; } = null;

	[JsonProperty(Order = 0)]
	public List<Comment> Comments = new();
	
	[JsonProperty(Order = 0)]
	public List<Post> Posts = new();
}