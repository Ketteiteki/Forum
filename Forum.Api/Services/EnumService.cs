using Forum.Api.Interfaces;

namespace Forum.Api.Services;

public class EnumService : IEnumService
{
	public bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct
	{
		try
		{
			result = Enum.Parse<TEnum>(value);
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}
}