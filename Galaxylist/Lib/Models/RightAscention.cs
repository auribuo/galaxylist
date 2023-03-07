namespace Galaxylist.Lib.Models;

/// <summary>
/// Represents the right ascention of an object in the sky in hours, minutes and seconds.
/// This is used in the equatorial coordinate system. See <see cref="EquatorialCoordinate"/>
/// </summary>
public struct RightAscention
{
	/// <summary>
	/// The hours part of the right ascention
	/// </summary>
	public double Hours { get; init; }
	
	/// <summary>
	/// The minutes part of the right ascention
	/// </summary>
	public double Minutes { get; init; }
	
	/// <summary>
	/// The seconds part of the right ascention
	/// </summary>
	public double Seconds { get; init; }
	
	/// <summary>
	/// Converts the right ascention in hours, minutes and seconds to its decimal representation.
	/// </summary>
	public double ToDegree()
	{
		return (Hours * 3600 + Minutes * 60 + Seconds) * 360 / 86400;
	}
}