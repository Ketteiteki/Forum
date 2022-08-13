using System.Text;
using Forum.Api.Interfaces;
using Forum.Api.Middlewares;
using Forum.Api.Services;
using Forum.BackendServices.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Forum.Api;

public class Startup
{
	public IConfiguration Configuration { get; set; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}
	
	public void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddDbContext<DatabaseContext>(options =>
		{
			options.UseNpgsql(Configuration.GetConnectionString("DefaultConnections"));
		});
		
		serviceCollection.AddControllers().AddNewtonsoftJson(options =>
		{
			options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			options.SerializerSettings.Converters.Add(new StringEnumConverter());
		});
		
		serviceCollection.AddEndpointsApiExplorer();
		
		serviceCollection.AddSwaggerGen(c =>
		{
			c.AddSecurityDefinition(
				"token",
				new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer",
					In = ParameterLocation.Header,
					Name = HeaderNames.Authorization
				}
			);
			
			c.AddSecurityRequirement(
				new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "token"
							},
						},
						Array.Empty<string>()
					}
				}
			);
		});

		serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(jwtBearerOptions =>
			{
				jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = false,
					ValidateIssuer = false,
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Configuration["JwtSettings:SecretAccessKey"])),
					ClockSkew = TimeSpan.Zero
				};
			});
		serviceCollection.AddAuthorization();

		serviceCollection.AddScoped<IAuthService, AuthService>();
		serviceCollection.AddScoped<ITokenService, TokenService>();
		serviceCollection.AddScoped<IMailService, MailService>();
		serviceCollection.AddScoped<IPostService, PostService>();
		serviceCollection.AddScoped<ICommentService, CommentService>();
		serviceCollection.AddScoped<IFileService, FileService>();
		serviceCollection.AddScoped<IGuidService, GuidService>();
		serviceCollection.AddScoped<IUserService, UserService>();
		serviceCollection.AddScoped<IAdminService, AdminService>();
		serviceCollection.AddScoped<IEnumService, EnumService>();
	}

	public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment environment)
	{
		if (environment.IsDevelopment())
		{
			applicationBuilder.UseSwagger();
			applicationBuilder.UseSwaggerUI();
		}

		applicationBuilder.UseStaticFiles();
		
		applicationBuilder.UseHttpsRedirection();
		
		applicationBuilder.UseRouting();
		
		applicationBuilder.UseAuthentication();
		applicationBuilder.UseAuthorization();
		
		applicationBuilder.UseMiddleware<ExceptionsMiddleware>();

		applicationBuilder.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}