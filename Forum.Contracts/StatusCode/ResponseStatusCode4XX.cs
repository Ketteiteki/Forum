namespace Forum.Contracts.StatusCode;

public class ResponseStatusCode4XX
{
	public string Message { get; set; }

	public ResponseStatusCode4XX(string message) => Message = message;
}