namespace Galaxylist.Models;

public class Location
{
	public required string Longitude { get; set; }
	public required string Latitude { get; set; }
	public int? Altitude { get; set; }
}