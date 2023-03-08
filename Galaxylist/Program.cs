global using FastEndpoints;
global using Galaxylist.Lib.Data;
global using Galaxylist.Lib.Data.Ugc;
global using Galaxylist.Lib.Models;
global using Galaxylist.Lib.Filter;
global using Galaxylist.Lib.Filter.Absolute;
global using Galaxylist.Lib.Extensions;
using FastEndpoints.Swagger;
using Galaxylist.Features.V1.Galaxies;

namespace Galaxylist;

/// <summary>
/// Entry point of the application
/// </summary>
public static class Program
{
	/// <summary>
	/// Entry point of the application
	/// </summary>
	/// <param name="args">Command line arguments</param>
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
		builder.Services.AddFastEndpoints(); // auto-discover endpoints
		builder.Services.AddCors();
		builder.Services.AddSwaggerDoc(swaggerSettings =>
									   {
										   swaggerSettings.Title = "Galaxylist";
										   swaggerSettings.Version = "v1";
									   },
									   shortSchemaNames: true
		);

		WebApplication app = builder.Build();
		app.UseAuthorization();
		app.UseFastEndpoints(config =>
			{
				// route options and docs options
				config.Endpoints.RoutePrefix = "api";
				config.Endpoints.ShortNames = true;
			}
		);

		app.UseCors(x => x.AllowAnyMethod()
						  .AllowAnyHeader()
						  .AllowAnyOrigin()
		);

		app.UseSwaggerGen();

		app.Run();
	}
}