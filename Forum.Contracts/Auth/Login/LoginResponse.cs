namespace Forum.Contracts.Auth.Login;

public class LoginResponse
{
	public string RefreshToken { get; set; }
	
	public string AccessToken { get; set; }
	
	public Guid Id { get; set; }
	
	public string Login { get; set; }
	
	public string? AvatarLink { get; set; }
	
	public string Role { get; set; }

	public LoginResponse(Guid id, string login, string? avatarLink, string role, string accessToken, string refreshToken)
	{
		RefreshToken = refreshToken;
		AccessToken = accessToken;
		Id = id;
		Login = login;
		AvatarLink = avatarLink;
		Role = role;
	}
}