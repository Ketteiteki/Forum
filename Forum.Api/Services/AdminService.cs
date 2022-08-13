using Forum.Api.Exceptions;
using Forum.Api.Interfaces;
using Forum.BackendServices.Database;
using Forum.BackendServices.Entities;
using Forum.BackendServices.Entities.Enums;

namespace Forum.Api.Services;

public class AdminService : IAdminService
{
	private readonly DatabaseContext _context;

	public AdminService(DatabaseContext contex)
	{
		_context = contex;
	}
	
	public async Task<User> BanUserAsync(Guid userId, DateTime dateTime)
	{
		var findUser = _context.Users.FirstOrDefault(u => u.Id == userId);
		if (findUser == null) throw new SimpleDbEntityNotFoundException("Пользователь не найден");

		findUser.IsBanned = true;
		findUser.BanExpires = dateTime;
		await _context.SaveChangesAsync();

		return findUser;
	}

	public async Task<User> SetRoleAsync(User user, Roles role)
	{
		user.Role = role.ToString();
		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		return user;
	}
}