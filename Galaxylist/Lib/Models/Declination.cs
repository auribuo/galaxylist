namespace Galaxylist.Lib.Models;

public struct Declination
{
	public double Degrees { get; set; }
	public double Minutes { get; set; }
	public double Seconds { get; set; }

	/// <summary>
	/// Konvertiert Grad, Gradminuten und Gradsekunden in Grad um
	/// </summary>
	public double ToDegrees()
	{
		if (Degrees < 0)
		{
			return Degrees - (Minutes / 60 + Seconds / 3600);
		}

		return Degrees + Minutes / 60 + Seconds / 3600;
	}
}