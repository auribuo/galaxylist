namespace Galaxylist.Features.V1.Galaxies;

/// <summary>
/// Request schema for the <see cref="GalaxyFilterEndpoint"/> endpoint. It supplies the necessary information to filter the galaxies.
/// </summary>
public class GalaxyFilterRequest
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
	/// Minimum height of the object in degrees in the azimuthal coordinate system it has to have to be considered
	/// </summary>
	public required int MinimumHeight { get; set; }
	
	/// <summary>
	/// The hemisphere the observer wants to observe. Needed to avoid meridian flip. Either "E" or "W"
	/// </summary>
	public required string Hemisphere { get; set; }
}