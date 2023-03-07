namespace Galaxylist.Lib.Models;

/// <summary>
/// Represents a coordinate in the azimuthal coordinate system
/// </summary>
public struct AzimuthalCoordinate
{
	/// <summary>
	/// The height of the object in the sky based on the observer's location and time starting from the horizon.
	/// </summary>
	public double Height { get; init; }

	/// <summary>
	/// The azimuth of the object in the sky based on the observer's location and time starting from the north.
	/// </summary>
	public double Azimuth { get; init; }
}