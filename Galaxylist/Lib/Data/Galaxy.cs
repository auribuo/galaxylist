namespace Galaxylist.Lib.Data;

/// <summary>
/// Data model of a single galaxy.
/// </summary>
public class Galaxy
{
	/// <summary>
	/// The hubbles type of the galaxy. May not be present in the catalog and thus null.
	/// </summary>
	public string? HubbleType { get; set; }

	/// <summary>
	/// The UGC number of the galaxy.
	/// </summary>
	public required int UgcNumber { get; set; }

	/// <summary>
	/// The magnitude of the galaxy.
	/// </summary>
	public required double Magnitude { get; set; }

	/// <summary>
	/// The equatorial coordinate of the galaxy.
	/// </summary>
	public required EquatorialCoordinate EquatorialCoordinate { get; init; }

	/// <summary>
	/// The azimuthal coordinate of the galaxy. May be null because it can be calculated only if the location and time are known.
	/// </summary>
	public AzimuthalCoordinate? AzimuthalCoordinate { get; set; }

	/// <summary>
	/// The semi major axis of the galaxy.
	/// </summary>
	public required double SemiMajorAxis { get; set; }

	/// <summary>
	/// The semi minor axis of the galaxy.
	/// </summary>
	public double SemiMinorAxis { get; set; }

	/// <summary>
	/// The position angle of the galaxy. May not be present in the catalog and thus -1.
	/// </summary>
	public double PositionAngle { get; set; }
}