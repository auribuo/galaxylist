namespace Galaxylist.Features.V1.Galaxies;

/// <summary>
/// Class that represents the response of the <see cref="GalaxyEndpoint"/> endpoint. It contains the total number of galaxies and the list of galaxies parsed from the catalog.
/// The galaxies are not calculated and have no azimuthal coordinates because they are location and time dependent.
/// </summary>
public class GalaxyResponse
{
	/// <summary>
	/// Total number of galaxies in the catalog
	/// </summary>
	public required int Total { get; set; }
	
	/// <summary>
	/// Galaxies parsed from the catalog
	/// </summary>
	public required List<Galaxy> Galaxies { get; set; }
	
	
	public List<Viewport> Viewports { get; set; }
}