namespace Galaxylist.Lib.Data;

using Models;

public class Galaxy
{
	public string? HubbleType { get; set; }
	public required int UgcNumber { get; set; }
	public required double Magnitude { get; set; }

	public required EquatorialCoordinate EquatorialCoordinate { get; set; }

	public AzimuthalCoordinate? AzimuthalCoordinate { get; set; }
	public required double SemiMajorAxis { get; set; }
	public double SemiMinorAxis { get; set; }
	public double PositionAngle { get; set; }
}