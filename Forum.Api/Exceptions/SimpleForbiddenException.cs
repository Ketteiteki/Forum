namespace Forum.Api.Exceptions;

public class SimpleForbiddenException : Exception
{
	public SimpleForbiddenException(string message) : base(message) {}
}