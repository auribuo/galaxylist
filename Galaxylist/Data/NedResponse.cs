namespace Galaxylist.Lib.Data;

public class NedResponse
{
	public required int UgcNumber { get; set; }

	public required string PreferredName { get; set; }
	public required double Magnitude { get; set; }
	public required double Redshift { get; set; }
	public required string HubbleType { get; set; }
}