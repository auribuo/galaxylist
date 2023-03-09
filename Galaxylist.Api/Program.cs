global using FastEndpoints;
global using FastEndpoints.Swagger;
global using Microsoft.AspNetCore.Http;

//using Galaxylist.Lib.Workbook;

namespace Galaxylist.Api;

using System.Diagnostics;
using System.Text.Json.Serialization;
using Data.Repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Processors;

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
				config.Endpoints.Configurator = cfg =>
				{
					cfg.PreProcessors(Order.Before, new TimePreProcessor());
					cfg.PostProcessors(Order.Before, new TimePostProcessor());
				};
			}
		);

		app.UseCors(x => x.AllowAnyMethod()
						  .AllowAnyHeader()
						  .AllowAnyOrigin()
		);

		app.UseSwaggerGen();
		
		ILogger<GalaxyDataRepo>? logger = app.Services.GetRequiredService<ILogger<GalaxyDataRepo>>();
		logger.Log(LogLevel.Information, "Initializing data repo...");
		Stopwatch stopwatch = new();
		stopwatch.Start();
		GalaxyDataRepo.Init(logger);
		stopwatch.Stop();
		logger.Log(LogLevel.Information, "Data repo initialized in {0}ms", stopwatch.ElapsedMilliseconds);
		
		//CalculateNeighbours.viewports();

		/*DateTime time = new DateTime(2023,03,9,1,0,0);

		EquatorialCoordinate coord = new EquatorialCoordinate{Declination = new Declination{Degrees = 90,Minutes = 0,Seconds = 0}, RightAscention = new RightAscention{Hours =1,Minutes = 0,Seconds = 0}};
		coord.ToAzimuthal(time, new Location{Latitude = 47,Longitude = 12});
		*/
		
		app.Run();
	}
}