namespace Galaxylist.Lib.Data.Ugc;

using Models;

public class UgcDataRepo
{
	private UgcDataRepo() { }

	public static UgcDataRepo New() => new();

	private List<UgcResponse> _ugcResponses = new();

	private readonly List<KeyValuePair<string, string>> _formFields = new()
	{
		Kv("-ref", "VIZ64060aaa23fbb0"),
		Kv("-to", "4"),
		Kv("-from", "-3"),
		Kv("-this", "-3"),
		Kv("//source", "VII/26D"),
		Kv("//tables", "VII/26D/catalog"),
		Kv("//tables", "VII/26D/errors"),
		Kv("-out.max", "unlimited"),
		Kv("//CDSportal", "http://cdsportal.u-strasbg.fr/StoreVizierData.html"),
		Kv("-out.form", "ascii+text/plain"),
		Kv("-out.add", "_RAJ,_DEJ"),
		Kv("//outaddvalue", "default"),
		Kv("-order", "I"),
		Kv("-oc.form", "sexa"),
		Kv("-out.src", "VII/26D/catalog,VII/26D/errors"),
		Kv("-nav", "cat:VII/26D&tab:{VII/26D/catalog}&tab:{VII/26D/errors}&key:source=VII/26D&HTTPPRM:&"),
		Kv("-c", ""),
		Kv("-c.eq", "J2000"),
		Kv("-c.r", "++2"),
		Kv("-c.u", "arcmin"),
		Kv("-c.geom", "r"),
		Kv("-source", ""),
		Kv("-source", "VII/26D/catalog VII/26D/errors"),
		Kv("-out", "MajAxis"),
		Kv("-out", "MinAxis"),
		Kv("-out", "PA"),
		Kv("-out", "Hubble"),
		Kv("-out", "Pmag"),
		Kv("-out", "i"),
		Kv("-meta.ucd", "1"),
		Kv("-meta", "1"),
		Kv("-meta.foot", "1"),
		Kv("-usenav", "1"),
		Kv("-bmark", "POST"),
	};

	private static KeyValuePair<string, string> Kv(string key, string value)
	{
		return new KeyValuePair<string, string>(key, value);
	}

	public async Task<UgcDataRepo> FetchAsync()
	{
		using HttpClient cli = new();
		HttpResponseMessage response =
			await cli.PostAsync("https://vizier.unistra.fr/viz-bin/asu-txt", new FormUrlEncodedContent(_formFields));

		string content = await response.Content.ReadAsStringAsync();
		_ugcResponses = Parse(content)
			.ToList();

		return this;
	}

	public IEnumerable<Galaxy> Galaxies => _ugcResponses.Select(MapGalaxy);

	public IEnumerable<Galaxy> HubbleTypedGalaxies =>
		_ugcResponses.Where(response => !string.IsNullOrWhiteSpace(response.Hubble))
					 .Select(MapGalaxy);

	private static Galaxy MapGalaxy(UgcResponse response) =>
		new()
		{
			RightAscension = response.Ra!.Value,
			Declination = response.Dec!.Value,
			Magnitude = response.PMag,
			PositionAngle = response.Pa,
			SemiMajorAxis = response.MajAxis,
			SemiMinorAxis = response.MinAxis,
		};

	private static IEnumerable<UgcResponse> Parse(string content) =>
		content.Split(Environment.NewLine)
			   .Where(line => line != string.Empty)
			   .SkipWhile(line => !line.StartsWith("--------"))
			   .Skip(1)
			   .SkipWhile(line => !line.StartsWith("--------"))
			   .SkipOut(1, out string header)
			   .TakeWhile(line => !line.StartsWith("#"))
			   .Select(line => ParseLine(line, header));

	private static UgcResponse ParseLine(string line, string header)
	{
		string?[] tokens = line.SliceAccordingTo(header.Split(" ")
												       .Select(tok => tok.Length)
							   )
							   .Select(l => l is not null ? l.Trim() : l)
							   .ToArray();

		return new UgcResponse
		{
			Ra = new RightAscention
			{
				H = Convert.ToDouble(tokens[0]!.Split(" ")[0]),
				M = Convert.ToDouble(tokens[0]!.Split(" ")[1]),
				S = Convert.ToDouble(tokens[0]!.Split(" ")[2]),
			},
			Dec = new Declination
			{
				D = Convert.ToDouble(tokens[1]!.Split(" ")[0]),
				M = Convert.ToDouble(tokens[1]!.Split(" ")[1]),
				S = Convert.ToDouble(tokens[1]!.Split(" ")[2]),
			},
			MajAxis = Convert.ToDouble(tokens[2]),
			MinAxis = Convert.ToDouble(tokens[3]),
			Pa = Convert.ToDouble(tokens[4] ?? "-1"),
			Hubble = tokens[5],
			PMag = Convert.ToDouble(tokens[6]),
			I = Convert.ToInt32(tokens[7] ?? "-1"),
		};
	}
}

class UgcResponse
{
	public RightAscention? Ra { get; set; }
	public Declination? Dec { get; set; }
	public double MajAxis { get; set; }
	public double MinAxis { get; set; }
	public double Pa { get; set; }
	public string? Hubble { get; set; }
	public double PMag { get; set; }
	public int I { get; set; }
}