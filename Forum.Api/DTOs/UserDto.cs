using Forum.BackendServices.Entities;

namespace Forum.Api.DTOs;

public class UserDto
{
	public Guid Id { get; set; }
	
	public string Login { get; set; }
	
	public string? AvatarLink { get; set; }
	
	public string Role { get; set; }
	
	public UserDto(User user)
	{
		Id = user.Id;
		Login = user.Login;
		AvatarLink = user.AvatarLink;
		Role = user.Role;
	}
}