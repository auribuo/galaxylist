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
		List<Galaxy> galaxyList = (strat switch
		{
			"alg" => GenericFilter.Filter(galaxies, new GalaxyRepo(), req.ObservationStart, 20)
								  .ToList(),
			"rng" => RandomFilter.Filter(galaxies),
			var _ => throw new ArgumentException($"Invalid strategy: {strat}", nameof(strat)),
		}).ToList();

		GalaxyDataRepo.Galaxies()
					  .ToList()
					  .ForEach(x => x.Reset());

		await SendAsync(new CalculateResponse
			{
				Total = galaxyList.Count,
				TotalQuality = galaxyList.Sum(g => g.Quality),
				Path = galaxyList,
				ViewportPath = null
			}
		);
	}
}