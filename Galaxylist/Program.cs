global using FastEndpoints;
global using Galaxylist.Lib.Data;
global using Galaxylist.Lib.Models;
using FastEndpoints.Swagger;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints(); // auto-discover endpoints
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

app.UseSwaggerGen();
app.Run();