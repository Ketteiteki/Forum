namespace Forum.Api.Extensions;

public static class EnumExtension
{
	public static TEnum? ToEnum<TEnum>(this string value) where TEnum : struct
	{
		if (Enum.TryParse<TEnum>(value, out var result))
			return result;
		
		return null;
	}
}