namespace Galaxylist.Features.V1.Calculate;

/// <summary>
/// Request schema for the <see cref="CalculateEndpoint"/> endpoint. It supplies the necessary information to calculate the ideal path of galaxies to take.
/// </summary>
public class CalculateRequest
{
	/// <summary>
	/// Observation start time. Will be automatically converted to UTC.
	/// </summary>
	public required DateTime ObservationStart { get; set; }

	/// <summary>
	/// Location of the observer
	/// </summary>
	public required Location Location { get; set; }

	/// <summary>
	/// Telescope information needed for the calculation of the area that can be observed
	/// </summary>
	public required Telescope Telescope { get; set; }

	/// <summary>
	/// Field of view of the camera mounted to the telescope
	/// </summary>
	public required Fov Fov { get; set; }

	/// <summary>
	/// The hemisphere the observer wants to observe. Needed to avoid meridian flip. Either "E" or "W"
	/// </summary>
	public required string Hemisphere { get; set; }

	/// <summary>
	/// Minimum height of the object in degrees in the azimuthal coordinate system it has to have to be considered
	/// </summary>
	public int MinimumHeight { get; set; } = 30;

	/// <summary>
	/// The max semi major axis of the galaxy in arcminutes
	/// </summary>
	public double MaxSemiMajorAxis { get; set; } = 10;

	/// <summary>
	///  The max semi minor axis of the galaxy in arcminutes
	/// </summary>
	public double MaxSemiMinorAxis { get; set; } = 10;
}