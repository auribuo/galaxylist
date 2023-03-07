namespace Galaxylist.Lib.Data.Ugc;

/// <summary>
/// Data repository for the UGC catalog. Fetches the catalog from the CDS and parses it into a list of galaxies.
/// </summary>
public class UgcDataRepo
{
	/// <summary>
	/// Constructor for the <see cref="UgcDataRepo"/>.
	/// Fetches the catalog from VizieR and parses it into a list of galaxies on the first call to <see cref="New"/>.
	/// The constructor is kept private to ensure that the catalog is only fetched once.
	/// </summary>
	private UgcDataRepo()
	{
		Task.WaitAll(FetchAsync());
	}

	/// <summary>
	/// Returns an instance of the <see cref="UgcDataRepo"/>. On the first call, the catalog is fetched from VizieR and saved locally.
	/// </summary>
	/// <returns>A instance of <see cref="UgcDataRepo"/></returns>
	public static UgcDataRepo New() => _instance ??= new UgcDataRepo();

	// instance variable
	private static UgcDataRepo? _instance;

	// repository data
	private List<UgcResponse> _ugcResponses = new();

	// formData for the VizieR request
	private readonly List<KeyValuePair<string, string>> _formFields = new()
	{
		("-ref", "VIZ64060aaa23fbb0").ToKvPair(),
		("-to", "4").ToKvPair(),
		("-from", "-3").ToKvPair(),
		("-this", "-3").ToKvPair(),
		("//source", "VII/26D").ToKvPair(),
		("//tables", "VII/26D/catalog").ToKvPair(),
		("//tables", "VII/26D/errors").ToKvPair(),
		("-out.max", "unlimited").ToKvPair(),
		("//CDSportal", "http://cdsportal.u-strasbg.fr/StoreVizierData.html").ToKvPair(),
		("-out.form", "ascii+text/plain").ToKvPair(),
		("-out.add", "_RAJ,_DEJ").ToKvPair(),
		("//outaddvalue", "default").ToKvPair(),
		("-order", "I").ToKvPair(),
		("-oc.form", "sexa").ToKvPair(),
		("-out.src", "VII/26D/catalog,VII/26D/errors").ToKvPair(),
		("-nav", "cat:VII/26D&tab:{VII/26D/catalog}&tab:{VII/26D/errors}&key:source=VII/26D&HTTPPRM:&").ToKvPair(),
		("-c", "").ToKvPair(),
		("-c.eq", "J2000").ToKvPair(),
		("-c.r", "++2").ToKvPair(),
		("-c.u", "arcmin").ToKvPair(),
		("-c.geom", "r").ToKvPair(),
		("-source", "").ToKvPair(),
		("-source", "VII/26D/catalog VII/26D/errors").ToKvPair(),
		("-out", "UGC").ToKvPair(),
		("-out", "MajAxis").ToKvPair(),
		("-out", "MinAxis").ToKvPair(),
		("-out", "PA").ToKvPair(),
		("-out", "Hubble").ToKvPair(),
		("-out", "Pmag").ToKvPair(),
		("-out", "i").ToKvPair(),
		("-meta.ucd", "1").ToKvPair(),
		("-meta", "1").ToKvPair(),
		("-meta.foot", "1").ToKvPair(),
		("-usenav", "1").ToKvPair(),
		("-bmark", "POST").ToKvPair(),
	};

	/// <summary>
	/// Sends a request to VizieR and saves the parsed response in <see cref="_ugcResponses"/>.
	/// </summary>
	/// <returns></returns>
	private async Task<UgcDataRepo> FetchAsync()
	{
		using HttpClient cli = new();

		// https://vizier.unistra.fr/viz-bin/VizieR?-source=VII/26D&-to=3
		HttpResponseMessage response =
			await cli.PostAsync("https://vizier.unistra.fr/viz-bin/asu-txt", new FormUrlEncodedContent(_formFields));

		string content = await response.Content.ReadAsStringAsync();
		_ugcResponses = Parse(content)
			.ToList();

		return this;
	}

	/// <summary>
	/// Returns an <see cref="IEnumerable{T}"/> of <see cref="Galaxy"/>s which maps the UGC results to the <see cref="Galaxy"/> model.
	/// </summary>
	public IEnumerable<Galaxy> Galaxies => _ugcResponses.Select(MapGalaxy);

	/// <summary>
	/// Returns an <see cref="IEnumerable{T}"/> of <see cref="Galaxy"/>s which maps the UGC results to the <see cref="Galaxy"/> model.
	/// Filters out galaxies without a Hubble type.
	/// </summary>
	public IEnumerable<Galaxy> HubbleTypedGalaxies =>
		_ugcResponses.Where(response => !string.IsNullOrWhiteSpace(response.HubbleType))
					 .Select(MapGalaxy);

	/// <summary>
	/// Maps a <see cref="UgcResponse"/> to a <see cref="Galaxy"/>.
	/// </summary>
	/// <param name="response">The ugc response to map</param>
	/// <returns>A new <see cref="Galaxy"/> with all given values set</returns>
	private static Galaxy MapGalaxy(UgcResponse response) =>
		new()
		{
			UgcNumber = response.UgcNumber,
			HubbleType = response.HubbleType,
			EquatorialCoordinate = new EquatorialCoordinate
			{
				RightAscention = response.RightAscention!.Value,
				Declination = response.Declination!.Value
			},
			Magnitude = response.Magnitude,
			PositionAngle = response.PositionAngle,
			SemiMajorAxis = response.MajorAxis,
			SemiMinorAxis = response.MinorAxis,
		};

	/// <summary>
	/// Parses the VizieR response into a list of <see cref="UgcResponse"/>s.
	/// The VizieR response is a text file containing metadata and the records we want.
	/// </summary>
	/// <param name="content">The content of the response as a string</param>
	/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="UgcResponse"/> parsed from the given content</returns>
	private static IEnumerable<UgcResponse> Parse(string content) =>
		content.Split(Environment.NewLine)
			   .Where(line => line != string.Empty)             // Remove empty lines
			   .SkipWhile(line => !line.StartsWith("--------")) // Skip metadata to the beginning of the first table header delimiter
			   .Skip(1)                                         // Skip the first table header delimiter
			   .SkipWhile(line => !line.StartsWith("--------")) // Skip the table header definitions
			   .SkipOut(1, out string[] header)                 // Skip the second table header delimiter and save the header
			   .TakeWhile(line => !line.StartsWith("#"))        // Take all lines until the end of the table
			   .Select(line => ParseLine(line, header[0]));     // Parse each line into a UgcResponse individually

	/// <summary>
	/// Parses a single line of the VizieR response into a <see cref="UgcResponse"/>.
	/// </summary>
	/// <param name="line">The full line to parse</param>
	/// <param name="headerDelimiter">The table header delimiter to calculate the column widths from</param>
	/// <returns></returns>
	private static UgcResponse ParseLine(string line, string headerDelimiter)
	{
		string[] columnDelimiters = headerDelimiter.Split(" ");                 // Split the header delimiter into single column delimiters
		IEnumerable<int> columnWidths = columnDelimiters.Select(d => d.Length); // Calculate the column widths from the column delimiters
		string?[] tokens = line.SliceAccordingTo(columnWidths)                  // Slice the line into tokens according to the column widths
							   .Select(l => l is not null ? l.Trim() : l)       // Trim the tokens to remove unnecessary whitespace
							   .ToArray();

		// Create a new UgcResponse and set all values from the token array
		return new UgcResponse
		{
			RightAscention = new RightAscention
			{
				Hours = Convert.ToDouble(tokens[0]!.Split(" ")[0]),
				Minutes = Convert.ToDouble(tokens[0]!.Split(" ")[1]),
				Seconds = Convert.ToDouble(tokens[0]!.Split(" ")[2]),
			},
			Declination = new Declination
			{
				Degrees = Convert.ToDouble(tokens[1]!.Split(" ")[0]),
				Minutes = Convert.ToDouble(tokens[1]!.Split(" ")[1]),
				Seconds = Convert.ToDouble(tokens[1]!.Split(" ")[2]),
			},
			UgcNumber = Convert.ToInt32(tokens[2]),
			MajorAxis = Convert.ToDouble(tokens[3]),
			MinorAxis = Convert.ToDouble(tokens[4]),
			PositionAngle = Convert.ToDouble(tokens[5] ?? "-1"),
			HubbleType = tokens[6],
			Magnitude = Convert.ToDouble(tokens[7]),
			Inclination = Convert.ToInt32(tokens[8] ?? "-1"),
		};
	}
}