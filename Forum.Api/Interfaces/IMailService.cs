namespace Forum.Api.Interfaces;

public interface IMailService
{
	Task SendConfirmationToEmailAsync(string toMail, Guid userId, Guid activationСode);
}