namespace Galaxylist.Lib.Data;

/// <summary>
/// Data model of a single galaxy.
/// </summary>
public class Galaxy
{
	/// <summary>
	/// The hubbles type of the galaxy. May not be present in the catalog and thus null.
	/// </summary>
	public required string HubbleType { get; set; }

	/// <summary>
	/// The UGC number of the galaxy.
	/// </summary>
	public required int UgcNumber { get; set; }

	/// <summary>
	/// The magnitude of the galaxy.
	/// </summary>
	public required double Magnitude { get; set; }

	/// <summary>
	/// The equatorial coordinate of the galaxy.
	/// </summary>
	public required EquatorialCoordinate EquatorialCoordinate { get; init; }

	/// <summary>
	/// The azimuthal coordinate of the galaxy. May be null because it can be calculated only if the location and time are known.
	/// </summary>
	public AzimuthalCoordinate? AzimuthalCoordinate { get; set; }

	/// <summary>
	/// The semi major axis of the galaxy.
	/// </summary>
	public required double SemiMajorAxis { get; set; }

	/// <summary>
	/// The semi minor axis of the galaxy.
	/// </summary>
	public double SemiMinorAxis { get; set; }

	/// <summary>
	/// The position angle of the galaxy. May not be present in the catalog and thus -1.
	/// </summary>
	public double PositionAngle { get; set; }

	/// <summary>
	/// The redshift of the galaxy.
	/// </summary>
	public required double Redshift { get; set; }

	/// <summary>
	/// The inclination of the galaxy. May not be present in the catalog and thus -1.
	/// </summary>
	public required int Inclination { get; set; }

	/// <summary>
	/// Returns the disance of the galaxy in Mpc. Calculated from the redshift. Negative if the redshift was not given.
	/// </summary>
	public double Distance => Redshift * 299792.458 / 70;

	/// <summary>
	/// The overall quality of the galaxy. Calculated based only on the properties of the galaxy itself.
	/// </summary>
	/// TODO: Implement quality calculation
	public double Quality => CalculateQuality();

	private double CalculateQuality()
	{
		double distance = Distance;
		int inclination = Inclination;
		double typeWeigth = 1 + HubbleType switch
		{
			"E"   => 0.09984127,
			"S0"  => 0.08793651,
			"Sa"  => 0.13253968,
			"Sb"  => 0.14587302,
			"Sbc" => 0.16936508,
			"Sc"  => 0.21619048,
			"Scd" => 0.19746032,
			var _ => 0.149887, // Average of all other types
		};

		double quality = typeWeigth;

		if (distance > 0)
		{
			quality *= 1 / (distance / 1000);
		}
		else
		{
			quality *= 1d / (100d / 1000d);
		}
		
		// TODO perfection
		return ((int)(quality * 100)) / 100d;
	}
}