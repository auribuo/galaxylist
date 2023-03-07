namespace Galaxylist.Lib.Models;

/// <summary>
/// Represents a camera's field of view
/// </summary>
public class Fov
{
	/// <summary>
	/// The width of the field of view in degrees
	/// </summary>
	public required int Width { get; set; }

	/// <summary>
	/// The height of the field of view in degrees
	/// </summary>
	public required int Height { get; set; }

	/// <summary>
	/// The position angle of the field of view in degrees where 0 means the north side of the camera is perpendicular to the zenith
	/// </summary>
	public int? PositionAngle { get; set; } = 0;
}