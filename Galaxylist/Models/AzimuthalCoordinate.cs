namespace Galaxylist.Models;

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

	/// <summary>
	/// Implicit conversion from a tuple to an <see cref="AzimuthalCoordinate"/>
	/// </summary>
	/// <param name="tuple">The tuplo to convert</param>
	/// <returns>A new <see cref="AzimuthalCoordinate"/> with matching values</returns>
	public static implicit operator AzimuthalCoordinate((double height, double azimuth) tuple) =>
		new()
		{
			Height = tuple.height,
			Azimuth = tuple.azimuth
		};
}