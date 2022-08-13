using System.ComponentModel.DataAnnotations;

namespace Forum.Contracts.Auth.Registration;

public class RegistrationRequest
{
	[EmailAddress]
	[Required]
	public string Email { get; set; } = null!;
	
	[StringLength(20, MinimumLength = 4, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Login { get; set; } = null!;
	
	[StringLength(30, MinimumLength = 7, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Password { get; set; } = null!;

	[Compare("Password", ErrorMessage = "Пароли не совпадают")]
	[Required]
	public string PasswordConfirm { get; set; } = null!;
}