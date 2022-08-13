namespace Forum.Api.Exceptions;

public class SimpleDbEntityNotFoundException : Exception
{
	public SimpleDbEntityNotFoundException(string message) : base(message) {}
}