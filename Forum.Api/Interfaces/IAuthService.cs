using Forum.Contracts.Auth.Authentication;
using Forum.Contracts.Auth.Login;
using Forum.Contracts.Auth.Registration;

namespace Forum.Api.Interfaces;

public interface IAuthService
{
	public Task<RegistrationResponse> RegistrationAsync(RegistrationRequest registrationRequest);

	public Task<LoginResponse> LoginAsync(LoginRequest loginRequest);

	public Task<AuthenticationResponse> AuthenticationAsync(string refreshToken);

	public Task ActivateEmailConfirmAsync(Guid userId, Guid code);
}