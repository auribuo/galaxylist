namespace Galaxylist.Lib.Models;

/// <summary>
/// Represents a location on the earth
/// </summary>
public class Location
{
	/// <summary>
	/// The longitude of the location in degrees where 0 is the prime meridian thus the east is positive and the west is negative
	/// </summary>
	public required int Longitude { get; set; }

	/// <summary>
	/// The latitude of the location in degrees where 0 is the equator thus the north pole is 90 and the south pole is -90
	/// </summary>
	public required int Latitude { get; set; }
}