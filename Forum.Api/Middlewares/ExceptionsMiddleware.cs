using Forum.Api.Exceptions;
using Forum.Contracts.StatusCode;

namespace Forum.Api.Middlewares;

public class ExceptionsMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionsMiddleware(RequestDelegate next) => _next = next;

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next.Invoke(context);
		}
		catch (SimpleValidationException e)
		{
			context.Response.StatusCode = 400;
			await context.Response.WriteAsJsonAsync(new ResponseStatusCode4XX(e.Message));
		}
		catch (SimpleAuthorizationException e)
		{
			context.Response.StatusCode = 401;
			await context.Response.WriteAsJsonAsync(new ResponseStatusCode4XX(e.Message));
		}
		catch (SimpleForbiddenException e)
		{
			context.Response.StatusCode = 403;
			await context.Response.WriteAsJsonAsync(new ResponseStatusCode4XX(e.Message));
		}
		catch (SimpleDbEntityNotFoundException e)
		{
			context.Response.StatusCode = 404;
			await context.Response.WriteAsJsonAsync(new ResponseStatusCode4XX(e.Message));
		}
		catch (Exception e)
		{
		 	context.Response.StatusCode = 500;
		 	await context.Response.WriteAsJsonAsync(e.Message);
		}
	}
}
