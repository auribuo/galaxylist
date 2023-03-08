namespace Galaxylist.Lib.Data.Repo;

using Pathoschild.Http.Client;

/// <summary>
/// Data repository for the UGC catalog. Fetches the catalog from the CDS and parses it into a list of galaxies.
/// </summary>
public partial class GalaxyDataRepo : IGalaxyDataRepo
{
	private static ILogger<GalaxyDataRepo>? _logger;

	/// <summary>
	/// Initializes the <see cref="Init"/>. On the first call, the catalog is fetched from VizieR and saved locally.
	/// Subsequent calls to <see cref="Repo"/> will do nothing.
	/// </summary>
	public static void Init(ILogger<GalaxyDataRepo> logger)
	{
		_logger = logger;
		Task.WaitAll(FetchAsync());
	}

	// repository data
	private static List<UgcResponse> _ugcResponses = new();
	private static List<NedResponse> _nedResponses = new();

	

	/// <summary>
	/// Returns an <see cref="IEnumerable{T}"/> of <see cref="Galaxy"/>s which maps the UGC results to the <see cref="Galaxy"/> model.
	/// </summary>
	public IEnumerable<Galaxy> Galaxies() =>
		_ugcResponses.Join(_nedResponses, response => response.UgcNumber, response => response.UgcNumber, MapGalaxy);

	/// <summary>
	/// Maps a <see cref="UgcResponse"/> to a <see cref="Galaxy"/>.
	/// </summary>
	/// <param name="response">The ugc response to map</param>
	/// <param name="detail">The detail records from the ned catalog</param>
	/// <returns>A new <see cref="Galaxy"/> with all given values set</returns>
	private static Galaxy MapGalaxy(UgcResponse response, NedResponse detail) =>
		new()
		{
			UgcNumber = response.UgcNumber,
			Morphology = NormalizeMorphology(detail.HubbleType),
			EquatorialCoordinate = new EquatorialCoordinate
			{
				RightAscention = response.RightAscention!.Value,
				Declination = response.Declination!.Value
			},
			Magnitude = response.Magnitude,
			PositionAngle = response.PositionAngle,
			SemiMajorAxis = response.MajorAxis,
			SemiMinorAxis = response.MinorAxis,
			Redshift = detail.Redshift,
			Inclination = response.Inclination,
		};
}