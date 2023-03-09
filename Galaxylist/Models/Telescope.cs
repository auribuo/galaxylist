namespace Galaxylist.Models;

/// <summary>
/// Represents data about a telescope
/// </summary>
public class Telescope
{
	/// <summary>
	/// The focal length of the telescope in millimeters
	/// </summary>
	public required int FocalLength { get; set; }

	/// <summary>
	/// The aperture of the telescope in millimeters
	/// </summary>
	public required int Aperture { get; set; }
}