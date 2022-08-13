using Forum.Api.Interfaces;

namespace Forum.Api.Services;

public class GuidService : IGuidService
{
	public bool TryStringConvertToGuid(string value, out Guid result)
	{
		try
		{
			result = new Guid(value);
			return true;
		}
		catch (Exception)
		{
			result = default;
			return false;
		}
	}
}