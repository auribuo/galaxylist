namespace Galaxylist.Features.V1.Galaxies;

using Models;

public class GalaxyResponse
{
	public required double Magnitude { get; set; }
	public required RightAscention RightAscension { get; set; }
	public required Declination Declination { get; set; }
	public required double SemiMajorAxis { get; set; }
	public double SemiMinorAxis { get; set; }
	public double PositionAngle { get; set; }
}