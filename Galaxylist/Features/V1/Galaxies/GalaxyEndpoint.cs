namespace Galaxylist.Features.V1.Galaxies;

using Lib.Data.Repo;

/// <summary>
/// Endpoint that returns a list of all used galaxies.
/// Conforms to the UGC catalog.
/// </summary>
public class GalaxyEndpoint : Endpoint<EmptyRequest, GalaxyResponse>
{
	private readonly IGalaxyDataRepo _repo;

	/// <summary>
	/// Injects the <see cref="IGalaxyDataRepo"/> service.
	/// </summary>
	/// <param name="repo">The injected service</param>
	public GalaxyEndpoint(IGalaxyDataRepo repo)
	{
		_repo = repo;
	}
	
	/// <summary>
	/// <inheritdoc cref="BaseEndpoint.Configure"/>
	/// </summary>
	public override void Configure()
	{
		Get("/galaxies");
		Description(endpoint =>
			{
				endpoint.Produces<GalaxyResponse>(200, "application/json");
			}
		);

		AllowAnonymous();
	}

	/// <summary>
	/// <inheritdoc cref="FastEndpoints.Endpoint{TRequest,TResponse}.HandleAsync"/>
	/// </summary>
	/// <param name="req">Request dto</param>
	/// <param name="ct">Cancellation token</param>
	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		int limit = Query<int>("limit", false);
		List<Galaxy> galaxies = _repo.Galaxies().ToList();

		if (limit != 0)
		{
			galaxies = galaxies.Take(limit)
							   .ToList();
		}

		await SendAsync(new GalaxyResponse
			{
				Total = galaxies.Count,
				Galaxies = galaxies,
			}
		);
	}
}