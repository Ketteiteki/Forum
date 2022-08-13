using System.ComponentModel.DataAnnotations;
using Forum.BackendServices.Entities.Abstractions;

namespace Forum.BackendServices.Entities;

public class RefreshToken : IEntityBase
{
	public Guid Id { get; set; } = new Guid();
	
	[Required]
	public string Token { get; set; }
	
	public Guid UserId { get; set; }
	
	public User User { get; set; }
}