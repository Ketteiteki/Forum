using Forum.Api.Interfaces;
using Forum.Contracts.Auth.Authentication;
using Forum.Contracts.Auth.Login;
using Forum.Contracts.Auth.Registration;
using Forum.Contracts.StatusCode;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IGuidService _guidService;
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService, IGuidService guidService)
	{
		_guidService = guidService;
		_authService = authService;
	}

	[ProducesResponseType(typeof(ResponseStatusCode4XX), 401)]
	[ProducesResponseType(typeof(AuthenticationResponse), 200)]
	[HttpGet("authorization")]
	public async Task<IActionResult> Authorization()
	{
		var refreshToken = Request.Cookies["RefreshToken"];
		if (refreshToken == null) return Unauthorized(new ResponseStatusCode4XX("Не найден токен обновления"));

		var authenticationResponse = await _authService.AuthenticationAsync(refreshToken);
		
		return Ok(authenticationResponse);
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 400)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 404)]
	[ProducesResponseType(typeof(AuthenticationResponse), 200)]
	[HttpGet("activation")]
	public async Task<IActionResult> Activation([FromQuery] string userId, [FromQuery] string code)
	{
		if (!_guidService.TryStringConvertToGuid(userId, out var userGuid) ||
		    !_guidService.TryStringConvertToGuid(code, out var codeGuid))
			return BadRequest(new ResponseStatusCode4XX("userId или code не являются Guid"));
		
		await _authService.ActivateEmailConfirmAsync(userGuid, codeGuid);
		return Ok();
	}
	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(RegistrationResponse), 201)]
	[HttpPost("registration")]
	public async Task<IActionResult> Registration([FromBody] RegistrationRequest registrationRequest)
	{
		var registrationResponse = await _authService.RegistrationAsync(registrationRequest);
		
		return Created("/api/auth/registration", registrationResponse);
	}

	
	[ProducesResponseType(typeof(ResponseStatusCode4XX), 403)]
	[ProducesResponseType(typeof(LoginResponse), 200)]
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
	{
		var loginResponse = await _authService.LoginAsync(loginRequest);
		
		Response.Cookies.Append("RefreshToken", loginResponse.RefreshToken);
		
		return Ok(loginResponse);
	}
}