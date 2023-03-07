global using FastEndpoints;
using Galaxylist.Util;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
WebApplication app = builder.Build();
app.UseAuthorization();
app.UseFastEndpoints(config =>
	{
		config.Endpoints.RoutePrefix = "api";
		config.Endpoints.ShortNames = true;
		config.Versioning.Prefix = "v";
		config.Versioning.DefaultVersion = 0;
		config.Versioning.PrependToRoute = true;
	}
);





app.Run();