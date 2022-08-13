using System.Text.RegularExpressions;
using Forum.Api.Interfaces;
using Forum.BackendServices.Database;
using Forum.BackendServices.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.Api.Services;

public class UserService : IUserService
{
	private readonly DatabaseContext _context;

	public UserService(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<User?> GetUserAsync(Guid id)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

		return user;
	}

	public async Task<List<User>> GetUsersAsync([FromQuery] string? search, int page, int count)
	{
		var users = search == null ? 
			await  _context.Users
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync() 
			: 
			await _context.Users
				.Where(u => Regex.IsMatch(u.Login, $"{search}"))
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync();

		return users;
	}

	public async Task<User> DeleteUserAsync(User user)
	{
		_context.Users.Remove(user);
		await _context.SaveChangesAsync();

		return user;
	}

	public async Task<User> BanUserAsync(User user, DateTime dateTime)
	{
		user.BanExpires = dateTime;
		user.IsBanned = true;
		
		_context.Users.Update(user);
		await _context.SaveChangesAsync();

		return user;
	}
}