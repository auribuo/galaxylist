namespace Galaxylist.Lib.Data.Repo;

public partial class GalaxyDataRepo
{
	private static string NormalizeMorphology(string? type)
	{
		if (type is null || string.IsNullOrWhiteSpace(type))
		{
			return "Unknown";
		}

		type = type.Replace(":", " ");
		type = type.Replace("?", "");

		if (type.StartsWith("E"))
		{
			return "E";
		}

		string[] importantTypes =
		{
			"S0",
			"Sa",
			"Sb",
			"Sbc",
			"Sc",
			"Scd",

			//"SBa",
			//"SBb",
			//"SBc",
			//"Sbc",
			//"Sbcd",
		};

		string? typeMatched = importantTypes.FirstOrDefault(t => type.StartsWith(t));

		return typeMatched ?? "Unknown";
	}
}