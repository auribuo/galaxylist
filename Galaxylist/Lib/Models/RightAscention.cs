namespace Galaxylist.Lib.Models;

public struct RightAscention
{
	public double Hours { get; set; }
	public double Minutes { get; set; }
	public double Seconds { get; set; }
	
	/// <summary>
	/// Konvertiert Stunden, Minuten und Sekunden in Grad
	/// </summary>
	public double ToDegree()
	{
		return (Hours * 3600 + Minutes * 60 + Seconds) * 360 / 86400;
	}
}