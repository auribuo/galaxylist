namespace Galaxylist.Features.V1.Calculate;

using Lib.Coordinates;
using Lib.Data.Ugc;
using Lib.Filter.Absolute;

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
		Post("/calculate");
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
		FilterPipeline<Galaxy> pipeline = FilterPipeline<Galaxy>.New()
																.With(new PositionFilter(req.MinimumHeight))
																.With(new MeridianFilter(req.Hemisphere));

		IEnumerable<Galaxy> galaxies = UgcDataRepo.New()
												  .Galaxies.Select(g =>
													  {
														  g.AzimuthalCoordinate =
															  CoordinateConverter.ConvEqToZen(
																  req.ObservationStart,
																  req.Location,
																  g.EquatorialCoordinate
															  );

														  return g;
													  }
												  );

		List<Galaxy> galaxyList = pipeline.Filter(galaxies)
										  .ToList();

		await SendAsync(new CalculateResponse
			{
				Total = galaxyList.Count,
				Galaxies = galaxyList,
			}
		);
	}
}