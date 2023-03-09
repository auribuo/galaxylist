namespace Galaxylist.Api.Features.V1.Galaxies;

using System.Diagnostics;
using FastEndpoints;
using Galaxylist.Lib.Data.Repo;
using Lib.Data;
using Lib.Extensions;
using Lib.Filter.Absolute;

/// <summary>
/// Endpoint that returns a list of all used galaxies.
/// The galaxies are filtered through the absolute filters based on the request.
/// </summary>
public class GalaxyFilterEndpoint : Endpoint<GalaxyFilterRequest, GalaxyResponse>
{
	/// <summary>
	/// <inheritdoc cref="BaseEndpoint.Configure"/>
	/// </summary>
	public override void Configure()
	{
		Post("/galaxies");
		Description(endpoint =>
			{
				endpoint.Accepts<GalaxyFilterRequest>("application/json")
						.Produces<GalaxyResponse>(200, "application/json");
			}
		);

		AllowAnonymous();
	}

	/// <summary>
	/// <inheritdoc cref="FastEndpoints.Endpoint{TRequest,TResponse}.HandleAsync"/>
	/// </summary>
	/// <param name="req">Request dto</param>
	/// <param name="ct">Cancellation token</param>
	public override async Task HandleAsync(GalaxyFilterRequest req, CancellationToken ct)
	{
		int limit = Query<int>("limit", false);
		FilterPipeline<Galaxy> pipeline = FilterPipeline<Galaxy>.New()
																.With(new PositionFilter(req.MinimumHeight))
																.With(new MeridianFilter(req.Hemisphere))
																.With(new SizeFilter(req.MaxSemiMajorAxis, req.MaxSemiMinorAxis));

		Stopwatch stopwatch = new();
		stopwatch.Start();
		IEnumerable<Galaxy> galaxies = GalaxyDataRepo.Galaxies()
													 .Select(g =>
														 {
															 g.AzimuthalCoordinate =
																 g.EquatorialCoordinate.ToAzimuthal(req.ObservationStart, req.Location);

															 return g;
														 }
													 );

		List<Galaxy> galaxyList = pipeline.Filter(galaxies)
										  .ToList();

		stopwatch.Stop();
		Console.WriteLine($"Filtering took {stopwatch.ElapsedMilliseconds}ms");

		if (limit != 0)
		{
			galaxyList = galaxyList.Take(limit)
								   .ToList();
		}

		GalaxyResponse ret = new()
		{
			Total = galaxyList.Count,
			Galaxies = galaxyList,
		};

		if (req.SendViewports)
		{
			ret.Viewports = galaxyList.CalculateViewports(req.Fov, req.RasterApprox);
		}

		await SendAsync(ret);
	}
}