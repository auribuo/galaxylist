namespace Galaxylist.Api.Features.Calculate;

using Data.Repo;
using Extensions;
using FastEndpoints;
using Filter;
using Models;

/// <summary>
/// Endpoint that calculates the ideal path of galaxies to take based on <see cref="CalculateRequest"/>
/// </summary>
public class CalculateEndpoint : Endpoint<CalculateRequest, CalculateResponse>
{
	/// <summary>
	/// <inheritdoc cref="BaseEndpoint.Configure"/>
	/// </summary>
	public override void Configure()
	{
		Post("/calculate/{strat}");
		Description(endpoint =>
			{
				endpoint.Accepts<CalculateRequest>("application/json")
						.Produces<CalculateResponse>(200, "application/json");
			}
		);

		AllowAnonymous();
	}

	/// <summary>
	/// <inheritdoc cref="FastEndpoints.Endpoint{TRequest,TResponse}.HandleAsync"/>
	/// </summary>
	/// <param name="req">Request dto</param>
	/// <param name="ct">Cancellation token</param>
	public override async Task HandleAsync(CalculateRequest req, CancellationToken ct)
	{
		string strat = Route<string>("strat")!;
		var galaxies = GalaxyDataRepo.Galaxies()
									 .Select(g =>
										 {
											 g.AzimuthalCoordinate = g.EquatorialCoordinate.ToAzimuthal(req.ObservationStart, req.Location);

											 return g;
										 }
									 );

		galaxies = SituationalFilter.Filter(galaxies, req.Hemisphere, req.MinimumHeight, req.MaxSemiMajorAxis, req.MaxSemiMinorAxis);
		var galaxyList = galaxies.ToList();
		var itemList = galaxyList.ToList<IRatableObject>();

		if (req.SendViewports)
		{
			itemList = galaxyList.CalculateViewports(req.Fov, req.Location, req.ObservationStart)
								 .ToList<IRatableObject>();
		}

		List<IRatableObject> filteredResults = (strat switch
		{
			"alg" => GenericFilter.Filter(itemList, req)
								  .ToList(),
			"rng" => RandomFilter.Filter(itemList),
			var _ => throw new ArgumentException($"Invalid strategy: {strat}", nameof(strat)),
		}).ToList();

		GalaxyDataRepo.Galaxies()
					  .ToList()
					  .ForEach(x => x.Reset());

		Console.WriteLine(filteredResults[0]
						  .GetType()
						  .Name
		);

		await SendAsync(new CalculateResponse
			{
				Total = filteredResults.Count,
				TotalQuality = filteredResults.Sum(g => g.Quality()),
				TotalExposure = filteredResults.Sum(x => x.Exposure()),
				MaxSearchSeconds = req.MaxSearchSeconds,
				ExposureDeviation = req.MaxSearchSeconds - filteredResults.Sum(x => x.Exposure()),
				GalaxyPath = filteredResults[0] switch
				{
					Galaxy => filteredResults.Cast<Galaxy>()
											 .ToList(),
					var _ => new List<Galaxy>(),
				},
				GalaxyPathViewports = filteredResults[0] switch
				{
					Galaxy => filteredResults.Cast<Galaxy>()
											 .CalculateViewports(req.Fov, req.Location, req.ObservationStart)
											 .ToList(),
					var _ => new List<Viewport>(),
				},
				ViewportPath = filteredResults[0] switch
				{
					Viewport => filteredResults.Cast<Viewport>()
											   .ToList(),
					var _ => new List<Viewport>(),
				}
			}
		);
	}
}