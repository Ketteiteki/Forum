namespace Forum.Api.Interfaces;

public interface IEnumService
{
	public bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct;
}