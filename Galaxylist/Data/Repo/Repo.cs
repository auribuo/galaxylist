namespace Galaxylist.Lib.Data.Repo;

using Pathoschild.Http.Client;

/// <summary>
/// Data repository for the UGC catalog. Fetches the catalog from the CDS and parses it into a list of galaxies.
/// </summary>
public partial class GalaxyDataRepo
{
	private GalaxyDataRepo() { }

	private static ILogger<GalaxyDataRepo>? _logger;
	private static bool _isInit;

	/// <summary>
	/// Initializes the <see cref="Init"/>. On the first call, the catalog is fetched from VizieR and saved locally.
	/// Subsequent calls to <see cref="Repo"/> will do nothing.
	/// </summary>
	public static void Init(ILogger<GalaxyDataRepo> logger)
	{
		_logger = logger;
		Task.WaitAll(FetchAsync());

		// fetch double ugc numbers from _ugcResponses
		List<int> doubleUgcNumbers = _ugcResponses.GroupBy(x => x.UgcNumber)
												  .Where(x => x.Count() > 1)
												  .Select(x => x.Key)
												  .ToList();

		// remove double ugc numbers from _ugcResponses
		_galaxies = _ugcResponses.Where(x => !doubleUgcNumbers.Contains(x.UgcNumber))
								 .OrderBy(x => x.UgcNumber)
								 .Join(_nedResponses, ugc => ugc.UgcNumber, ned => ned.UgcNumber, MapGalaxy)
								 .ToDictionary(g => g.UgcNumber);

		_isInit = true;
	}

	// repository data
	private static List<UgcResponse> _ugcResponses = new();
	private static List<NedResponse> _nedResponses = new();

	/// <summary>
	/// Returns an <see cref="IEnumerable{T}"/> of <see cref="Galaxy"/>s which maps the UGC results to the <see cref="Galaxy"/> model.
	/// </summary>
	public static IEnumerable<Galaxy> Galaxies() => _galaxies.Select(x => x.Value);

	/// <summary>
	/// Gets a single galaxy by its UGC number.
	/// </summary>
	/// <param name="ugc"></param>
	/// <returns></returns>
	public static Galaxy Galaxy(int ugc) => _galaxies[ugc];

	private static Dictionary<int, Galaxy> _galaxies = new();

	/// <summary>
	/// Maps a <see cref="UgcResponse"/> to a <see cref="Galaxy"/>.
	/// </summary>
	/// <param name="response">The ugc response to map</param>
	/// <param name="detail">The detail records from the ned catalog</param>
	/// <returns>A new <see cref="Galaxy"/> with all given values set</returns>
	private static Galaxy MapGalaxy(UgcResponse response, NedResponse detail) =>
		new()
		{
			PreferredName = detail.PreferredName,
			UgcNumber = response.UgcNumber,
			Morphology = NormalizeMorphology(detail.HubbleType),
			EquatorialCoordinate = new EquatorialCoordinate
			{
				RightAscention = response.RightAscention!.Value,
				Declination = response.Declination!.Value,
			},
			Magnitude = detail.Magnitude > 0 ? detail.Magnitude : response.Magnitude,
			PositionAngle = response.PositionAngle,
			SemiMajorAxis = response.MajorAxis,
			SemiMinorAxis = response.MinorAxis,
			Redshift = detail.Redshift,
			Inclination = response.Inclination,
		};
}