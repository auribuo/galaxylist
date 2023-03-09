namespace Galaxylist.Lib.Data;

using Models;

/// <summary>
/// Response model for the UGC catalog.
/// </summary>
public class UgcResponse
{
	/// <summary>
	/// The UGC number of the galaxy.
	/// </summary>
	public int UgcNumber { get; init; }

	/// <summary>
	/// The right ascention of the galaxy.
	/// </summary>
	public RightAscention? RightAscention { get; init; }

	/// <summary>
	/// The declination of the galaxy.
	/// </summary>
	public Declination? Declination { get; init; }

	/// <summary>
	/// The major axis of the galaxy.
	/// </summary>
	public double MajorAxis { get; init; }

	/// <summary>
	/// The minor axis of the galaxy.
	/// </summary>
	public double MinorAxis { get; init; }

	/// <summary>
	/// The position angle of the galaxy. May not be present in the catalog.
	/// If not present, the position angle is set to -1.
	/// </summary>
	public double PositionAngle { get; init; }

	/// <summary>
	/// The type of the galaxy. May not be present in the catalog.
	/// </summary>
	public string? HubbleType { get; init; }

	/// <summary>
	/// The magnitude of the galaxy.
	/// </summary>
	public double Magnitude { get; init; }

	/// <summary>
	/// The inclination of the galaxy. May not be present in the catalog.
	/// If not present, the inclination is set to -1.
	/// </summary>
	public int Inclination { get; set; }
}