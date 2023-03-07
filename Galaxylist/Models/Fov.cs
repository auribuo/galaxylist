namespace Galaxylist.Models;

public class Fov
{
	public required int Width { get; set; }
	public required int Height { get; set; }
	public required int PositionAngle { get; set; } = 0;
}