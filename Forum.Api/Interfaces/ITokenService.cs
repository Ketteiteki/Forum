using System.IdentityModel.Tokens.Jwt;
using Forum.BackendServices.Entities;

namespace Forum.Api.Interfaces;

public interface ITokenService
{
	string CreateAccessToken(User user);

	string CreateRefreshToken(User user);

	JwtSecurityToken? ValidateAccessToken(string token);
	
	JwtSecurityToken? ValidateRefreshToken(string token);

	JwtSecurityToken ReadJwtToken(string token);
}