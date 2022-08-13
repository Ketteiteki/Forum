using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Forum.Api.Constants;
using Forum.Api.Interfaces;
using Forum.BackendServices.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Forum.Api.Services;

public class TokenService : ITokenService
{
	private readonly IConfiguration _configuration;

	public TokenService(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	
	public string CreateAccessToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.Default.GetBytes(_configuration["JwtSettings:SecretAccessKey"]);
		var token = new JwtSecurityToken(
			claims: new[] {
				new Claim(ClaimConstants.ID, user.Id.ToString()),
				new Claim(ClaimConstants.LOGIN, user.Login),
				new Claim(ClaimTypes.Role, user.Role),
				new Claim(ClaimConstants.ISBANNED ,user.IsBanned.ToString())
			},
			expires: DateTime.UtcNow.AddMinutes(300),
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
		
		return tokenHandler.WriteToken(token);
	}
	
	public string CreateRefreshToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.Default.GetBytes(_configuration["JwtSettings:SecretRefreshKey"]);
		var token = new JwtSecurityToken(
			claims: new[] {
				new Claim(ClaimConstants.ID, user.Id.ToString()),
				new Claim(ClaimConstants.LOGIN, user.Login),
				new Claim(ClaimTypes.Role, user.Role),
				new Claim(ClaimConstants.ISBANNED ,user.IsBanned.ToString())
			},
			expires: DateTime.UtcNow.AddDays(30),
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

		return tokenHandler.WriteToken(token);
	}

	public JwtSecurityToken? ValidateAccessToken(string token)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.Default.GetBytes(_configuration["JwtSettings:SecretAccessKey"]);
		try
		{
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			return (JwtSecurityToken)validatedToken;
		}
		catch
		{
			return null;
		}
	}
	
	public JwtSecurityToken? ValidateRefreshToken(string token)	
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.Default.GetBytes(_configuration["JwtSettings:SecretRefreshKey"]);
		try
		{
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			return (JwtSecurityToken)validatedToken;
		}
		catch
		{
			return null;
		}
	}

	public JwtSecurityToken ReadJwtToken(string token)
	{
		var jwtHandler = new JwtSecurityTokenHandler();
		return jwtHandler.ReadJwtToken(token);
	}
}