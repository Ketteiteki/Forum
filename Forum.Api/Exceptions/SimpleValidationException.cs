namespace Forum.Api.Exceptions;

public class SimpleValidationException : Exception
{
	public SimpleValidationException(string message) : base(message) {}
}