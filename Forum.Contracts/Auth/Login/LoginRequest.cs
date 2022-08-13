using System.ComponentModel.DataAnnotations;

namespace Forum.Contracts.Auth.Login;

public class LoginRequest
{
	[StringLength(20, MinimumLength = 4, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Login { get; set; } = null!;
	
	[StringLength(30, MinimumLength = 7, ErrorMessage = "Длина поля {0} должна быть от {2} до {1}")]
	[Required]
	public string Password { get; set; } = null!;
}