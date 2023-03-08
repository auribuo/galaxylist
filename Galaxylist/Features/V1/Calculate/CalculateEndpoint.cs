namespace Galaxylist.Features.V1.Calculate;

using Lib.Data.Repo;

/// <summary>
/// Endpoint that calculates the ideal path of galaxies to take based on <see cref="CalculateRequest"/>
/// </summary>
public class CalculateEndpoint : Endpoint<CalculateRequest, CalculateResponse>
{
	private readonly IGalaxyDataRepo _repo;

	/// <summary>
	/// Injects the <see cref="IGalaxyDataRepo"/> service.
	/// </summary>
	/// <param name="repo">The injected service</param>
	public CalculateEndpoint(IGalaxyDataRepo repo)
	{
		_repo = repo;
	}

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
		int limit = Query<int>("limit", false);
		FilterPipeline<Galaxy> pipeline = FilterPipeline<Galaxy>.New()
																.With(new PositionFilter(req.MinimumHeight))
																.With(new MeridianFilter(req.Hemisphere))
																.With(new SizeFilter());

		IEnumerable<Galaxy> galaxies = _repo.Galaxies()
											.Select(g =>
												{
													g.AzimuthalCoordinate =
														g.EquatorialCoordinate.ToAzimuthal(req.ObservationStart, req.Location);

													return g;
												}
											);

		List<Galaxy> galaxyList = pipeline.Filter(galaxies)
										  .ToList();

		if (limit != 0)
		{
			galaxyList = galaxyList.Take(limit)
								   .ToList();
		}

		await SendAsync(new CalculateResponse
			{
				Total = galaxyList.Count,
				Galaxies = galaxyList,
			}
		);
	}
}