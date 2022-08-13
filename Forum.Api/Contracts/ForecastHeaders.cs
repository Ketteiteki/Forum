using Microsoft.AspNetCore.Mvc;

namespace Forum.Api.Contracts;

public class ForecastHeaders
{
	[FromHeader]
	public string? Authorization { get; set; }
}