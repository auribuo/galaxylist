namespace Galaxylist.Features.V1.Galaxies;

using Lib.Data;
using Lib.Data.Ugc;

public class GalaxyEndpoint : Endpoint<EmptyRequest, List<GalaxyResponse>>
{
	public override void Configure()
	{
		Get("/galaxies");
		Version(1);
		AllowAnonymous();
	}

	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		UgcDataRepo repo = await UgcDataRepo.New()
											.FetchAsync();

		IEnumerable<Galaxy> galaxies = repo.Galaxies;
		await SendAsync(galaxies.Select(galaxy => new GalaxyResponse
									{
										Declination = galaxy.Declination,
										Magnitude = galaxy.Magnitude,
										PositionAngle = galaxy.PositionAngle,
										RightAscension = galaxy.RightAscension,
										SemiMajorAxis = galaxy.SemiMajorAxis,
										SemiMinorAxis = galaxy.SemiMinorAxis,
									}
								)
								.ToList()
		);
	}
}