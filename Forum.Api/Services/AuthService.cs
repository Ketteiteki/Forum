using Forum.Api.Constants;
using Forum.Api.Exceptions;
using Forum.Api.Extensions;
using Forum.Api.Interfaces;
using Forum.BackendServices.Database;
using Forum.BackendServices.Entities;
using Forum.Contracts.Auth.Authentication;
using Forum.Contracts.Auth.Login;
using Forum.Contracts.Auth.Registration;
using Microsoft.EntityFrameworkCore;

namespace Forum.Api.Services;

public class AuthService : IAuthService
{
	private readonly DatabaseContext _context;
	private readonly IMailService _mailService;
	private readonly ITokenService _tokenService;
	private readonly IConfiguration _configuration;

	public AuthService(DatabaseContext context, IMailService mailService, ITokenService tokenService, 
		IConfiguration configuration)
	{
		_context = context;
		_mailService = mailService;
		_tokenService = tokenService;
		_configuration = configuration;
	}

	public async Task<RegistrationResponse> RegistrationAsync(RegistrationRequest registrationRequest)
	{
		var findUserByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == registrationRequest.Email);
		if (findUserByEmail != null) throw new SimpleAuthorizationException("Почта уже занята");

		var findUserByLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login == registrationRequest.Login);
		if (findUserByLogin != null) throw new SimpleAuthorizationException("Логин уже занят");
		
		var newUser = new User
		{
			Email = registrationRequest.Email,
			Login = registrationRequest.Login,
			Password = registrationRequest.Password.ToHMACSHA512CryptoHash(_configuration["UserPasswordHashSalt"]),
		};
		
		_context.Users.Add(newUser);
		await _context.SaveChangesAsync();
		
		await _mailService.SendConfirmationToEmailAsync(newUser.Email, newUser.Id, newUser.ActivationCode);

		return new RegistrationResponse();
	}
	
	public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
	{
		var findUserByLogin = await _context.Users.FirstOrDefaultAsync(u => u.Login == loginRequest.Login);
		if (findUserByLogin == null) throw new SimpleAuthorizationException("Пользователя с таким логином не существует");

		if (loginRequest.Password.ToHMACSHA512CryptoHash(_configuration["UserPasswordHashSalt"]) != findUserByLogin.Password) 
			throw new SimpleAuthorizationException("Не верный пароль");
		
		if (findUserByLogin.EmailConfirm == false) throw new SimpleAuthorizationException("Почта не подтверждена");

		var findRefreshToken =
			await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == findUserByLogin.Id);

		if (findRefreshToken == null)
		{
			var refreshToken = new RefreshToken
			{
				Token =  _tokenService.CreateRefreshToken(findUserByLogin),
				User = findUserByLogin
			};
			_context.RefreshTokens.Add(refreshToken);
		}
		else
		{
			findRefreshToken.Token = _tokenService.CreateRefreshToken(findUserByLogin);
			_context.RefreshTokens.Update(findRefreshToken);
		}

		await _context.SaveChangesAsync();

		if (findUserByLogin.RefreshToken == null) throw new Exception("Ошибка сервера");
		
		var accessToken = _tokenService.CreateAccessToken(findUserByLogin);

		return new LoginResponse(findUserByLogin.Id, findUserByLogin.Login, findUserByLogin.AvatarLink,
			findUserByLogin.Role, accessToken, findUserByLogin.RefreshToken.Token);
	}
	
	public async Task<AuthenticationResponse> AuthenticationAsync(string refreshToken)
	{
		var validateToken = _tokenService.ValidateRefreshToken(refreshToken);
		if (validateToken == null) throw new SimpleAuthorizationException("Невалидный токен обновления");

		var claimId = validateToken.Claims.FirstOrDefault(c => c.Type == ClaimConstants.ID)?.Value;
		
		var findUser = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == claimId);
		if (findUser == null) throw new SimpleAuthorizationException("Некорректный токен обновления");

		if (findUser.BanExpires != null)
			if (findUser.BanExpires < DateTime.UtcNow)
			{
				findUser.IsBanned = false;
				findUser.BanExpires = null;
			}

		var findRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == findUser.Id);
		if (findRefreshToken == null) throw new Exception("Ошибка сервера");
		
		findRefreshToken.Token = _tokenService.CreateRefreshToken(findUser);
		_context.RefreshTokens.Update(findRefreshToken);
		await _context.SaveChangesAsync();
		
		var accessToken = _tokenService.CreateAccessToken(findUser);
		
		return new AuthenticationResponse(findUser.Id, findUser.Login, findUser.AvatarLink, findUser.Role, accessToken, findRefreshToken.Token);
	}

	public async Task ActivateEmailConfirmAsync(Guid userId, Guid code)
	{
		var findUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
		if (findUser == null) throw new SimpleDbEntityNotFoundException("Пользователь не найден");
		
		if (code != findUser.ActivationCode) throw new SimpleAuthorizationException("Неправильный код активации");

		findUser.EmailConfirm = true;
		await _context.SaveChangesAsync();
	}
}