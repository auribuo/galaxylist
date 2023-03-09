namespace Galaxylist.Models;

/// <summary>
/// Represents the declination of an object in the sky in degrees, minutes and seconds.
/// This is used in the equatorial coordinate system. See <see cref="EquatorialCoordinate"/>
/// </summary>
public struct Declination
{
	/// <summary>
	/// The degrees part of the declination
	/// </summary>
	public double Degrees { get; init; }
	
	/// <summary>
	/// The minutes part of the declination
	/// </summary>
	public double Minutes { get; init; }
	
	/// <summary>
	/// The seconds part of the declination
	/// </summary>
	public double Seconds { get; init; }

	/// <summary>
	/// Converts the declination in degrees, minutes and seconds to its decimal representation.
	/// </summary>
	public double ToDegrees()
	{
		if (Degrees < 0)
		{
			return Degrees - (Minutes / 60 + Seconds / 3600);
		}

		return Degrees + Minutes / 60 + Seconds / 3600;
	}
}