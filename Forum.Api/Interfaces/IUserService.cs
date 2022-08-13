using Forum.BackendServices.Entities;

namespace Forum.Api.Interfaces;

public interface IUserService
{
	Task<User?> GetUserAsync(Guid id);

	Task<List<User>> GetUsersAsync(string? search, int page, int count);

	Task<User> DeleteUserAsync(User user);
	
	Task<User> BanUserAsync(User user, DateTime dateTime);
}