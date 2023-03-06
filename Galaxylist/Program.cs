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

// 150 az
//CoordinateConverter.ConvEqToZen(DateTime.Today + new TimeSpan(21,0,0), 11.93, 46.8, 152.1, 11.9667);
CoordinateConverter.ConvEqToZen(DateTime.Today + new TimeSpan(17,0,0), 11.93, 46.8, 
	CoordinateConverter.HourMinSecToDegree(5,15,38.8), 
	CoordinateConverter.DegreeMinSecToDegree(-8,10,39)
	);

//app.Run();