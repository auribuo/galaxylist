namespace Galaxylist.Lib.Data.Ugc;

using Models;

public class UgcResponse
{
	public int UgcNumber { get; set; }
	public RightAscention? RightAscention { get; set; }
	public Declination? Declination { get; set; }
	public double MajorAxis { get; set; }
	public double MinorAxis { get; set; }
	public double PositionAlignment { get; set; }
	public string? HubbleType { get; set; }
	public double Magnitude { get; set; }
	public int Inclination { get; set; }
}