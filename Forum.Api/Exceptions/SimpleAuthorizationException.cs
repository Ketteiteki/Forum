namespace Forum.Api.Exceptions;

public class SimpleAuthorizationException : Exception
{
	public SimpleAuthorizationException(string message) : base(message) {}
}