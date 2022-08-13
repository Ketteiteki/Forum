using System.Net;
using System.Net.Mail;
using Forum.Api.Interfaces;

namespace Forum.Api.Services;

public class MailService : IMailService
{
	private readonly IConfiguration _configuration;

	public MailService(IConfiguration configuration)
	{
		_configuration = configuration;
	}
	
	public async Task SendConfirmationToEmailAsync(string toMail, Guid userId, Guid activationСode)
	{
		MailAddress from = new MailAddress(_configuration["MailSettings:Mail"], _configuration["MailSettings:DisplayName"]);
		MailAddress to = new MailAddress(toMail);
		var emailMessage = new MailMessage(from, to)
		{
			Subject = "Подтверждение почты",
			IsBodyHtml = true,
			Body = $"Чтобы подтвердить почту, перейдите по этой <a href='{_configuration["DomainName"]}/api/auth/activation?userId={userId}&code={activationСode}'>ссылке</a>"
		};
		
		SmtpClient smtp = new SmtpClient(_configuration["MailSettings:Smtp"], int.Parse(_configuration["MailSettings:SmtpPort"]))
		{
			Credentials = new NetworkCredential(_configuration["MailSettings:Mail"], _configuration["MailSettings:Password"]),
			EnableSsl = true
		};
		
		await smtp.SendMailAsync(emailMessage);
	}
}