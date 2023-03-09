namespace Galaxylist.Models;

/// <summary>
/// Represents a coordinate in the equatorial coordinate system.
/// The variant used is the J2000 system.
/// </summary>
public class EquatorialCoordinate
{
	/// <summary>
	/// The right ascention of the object in the sky.
	/// </summary>
	public RightAscention RightAscention { get; init; }

	/// <summary>
	/// The declination of the object in the sky.
	/// </summary>
	public Declination Declination { get; init; }
}