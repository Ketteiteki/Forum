using Forum.BackendServices.Entities;
using Forum.BackendServices.Entities.Enums;

namespace Forum.Api.Interfaces;

public interface IAdminService
{
	Task<User> SetRoleAsync(User user, Roles role);
}