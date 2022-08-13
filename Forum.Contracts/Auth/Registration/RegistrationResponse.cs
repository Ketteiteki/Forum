namespace Forum.Contracts.Auth.Registration;

public class RegistrationResponse
{
	public string Message { get; set; } = "Успешная регистрация";

	public RegistrationResponse(string message) => Message = message;
	
	public RegistrationResponse() {}
}