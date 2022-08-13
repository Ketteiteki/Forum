namespace Forum.Api;

class Program
{
	public static void Main(string[] args)
	{
		CreateWebHostBuilder(args).Build().Run();
	}

	public static IHostBuilder CreateWebHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});
}