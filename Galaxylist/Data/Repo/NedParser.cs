namespace Galaxylist.Lib.Data.Repo;

using Pathoschild.Http.Client;

public partial class GalaxyDataRepo
{
	private static async Task FetchDetailsAsync()
	{
		// https://ned.ipac.caltech.edu
		FluentClient cli = new("https://ned.ipac.caltech.edu");
		List<int[]> ugcNumbers = _ugcResponses.Select(r => r.UgcNumber)
											  .Chunk(500)
											  .ToList();

		IEnumerable<Task> tasks = ugcNumbers.Select(chunk => FetchDetailChunk(cli, chunk));
		await Task.WhenAll(tasks);
	}

	private static async Task FetchDetailChunk(IClient cli, IEnumerable<int> chunk)
	{
		List<int> chunkList = chunk.ToList();
		_logger!.Log(LogLevel.Information, "Fetching chunk from UGC{0} to UGC{1}", chunkList.First(), chunkList.Last());
		string response = await cli.GetAsync("cgi-bin/gmd")
								   .WithArguments(new List<KeyValuePair<string, string>>
									   {
										   ("uplist", UgcListString(chunkList)).ToKvPair(),
										   ("delimiter", "bar").ToKvPair(),
										   ("NO_LINKS", "1").ToKvPair(),
										   ("nondb", "user_objname").ToKvPair(),
										   ("crosid", "objname").ToKvPair(),
										   ("position", "z").ToKvPair(),
										   ("gadata", "magnit").ToKvPair(),
										   ("attdat_CON", "M").ToKvPair(),
										   ("attdat", "attned").ToKvPair(),
										   ("gphotoms", "q_value").ToKvPair(),
										   ("gphotoms", "q_unc").ToKvPair(),
										   ("gphotoms", "ned_value").ToKvPair(),
										   ("gphotoms", "ned_unc").ToKvPair(),
										   ("diamdat", "ned_maj_dia").ToKvPair(),
										   ("distance", "avg").ToKvPair(),
										   ("distance", "stddev_samp").ToKvPair(),
									   }
								   )
								   .AsString();

		_nedResponses = _nedResponses.Concat(ParseDetails(response))
									 .ToList();
	}

	private static string UgcListString(IEnumerable<int> ugcNumbers) =>
		ugcNumbers.Select(ugcNumber => $"UGC{ugcNumber}")
				  .Aggregate((acc, ugcNumber) => acc + $"\r{ugcNumber}");

	private static IEnumerable<NedResponse> ParseDetails(string content) =>
		content.Split("\n")
			   .SkipWhile(line => !line.StartsWith("UGC"))
			   .TakeWhile(line => line.StartsWith("UGC"))
			   .Select(line => line.Split("|"))
			   .Select(tokens => tokens.Select(token => token.Trim()))
			   .Select(tokens =>
				   {
					   List<string> recordFields = tokens.ToList();

					   return new NedResponse
					   {
						   UgcNumber = Convert.ToInt32(recordFields[0][3..]),
						   PreferredName = recordFields[1],
						   Redshift = recordFields[2] != "" ? Convert.ToDouble(recordFields[2]) : -1,
						   Magnitude = Convert.ToDouble(recordFields[3] == ""
														    ? "-1"
														    : recordFields[3]
														      .ToCharArray()
														      .Where(c => char.IsDigit(c) || c == '.')
														      .Aggregate("", (s, c) => s + c)
						   ),
						   HubbleType = recordFields[4],
					   };
				   }
			   );
}