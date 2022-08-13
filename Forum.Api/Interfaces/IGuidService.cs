namespace Forum.Api.Interfaces;

public interface IGuidService
{
	bool TryStringConvertToGuid(string value, out Guid result);
}