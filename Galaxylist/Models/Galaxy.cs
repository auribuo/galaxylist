namespace Galaxylist.Models;

using Dbscan;

/// <summary>
/// Data model of a single galaxy.
/// </summary>
public class Galaxy : JsonStringer, IPointData
{
	private const int MAG_UGC2 = 17;
	private const int DST_UGC2 = 268;
	private const int BASE_TIME_UGC2 = 536;

	private bool _visited;

	/// <summary>
	/// The morphology of the galaxy.
	/// </summary>
	public required string Morphology { get; set; }

	/// <summary>
	/// The UGC number of the galaxy.
	/// </summary>
	public required int UgcNumber { get; set; }

	/// <summary>
	/// The preferred name of the galaxy.
	/// </summary>
	public required string PreferredName { get; set; }

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
	/// Marks the galaxy as visited.
	/// This makes the galaxies value to be 0.
	/// </summary>
	public void Visit() => _visited = true;

	/// <summary>
	/// Resets the visited state of the galaxy.
	/// </summary>
	public void Reset() => _visited = false;

	/// <summary>
	/// The overall quality of the galaxy. Calculated based only on the properties of the galaxy itself.
	/// </summary>
	/// TODO: Implement quality calculation
	public double Quality => CalculateQuality();

	private double CalculateQuality()
	{
		double typeWeigth = 1 + Morphology switch
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

		double quality = typeWeigth * Math.Pow(10, (Magnitude - MAG_UGC2) / -2.5);

		return quality * Convert.ToInt32(!_visited);
	}

	private double ExposureTime(double baseTime) => baseTime * Math.Pow(DST_UGC2 / Distance, 2);

	private static double SlewFunction(double distance) => 1 / 2d * distance + 6;

	private const int READOUT_TIME = 3;

	/// <summary>
	/// Calculates the exposure time for a given adjustment angle in degrees.
	/// </summary>
	/// <param name="adjustmentAngle">The angle of the slew movement</param>
	/// <param name="baseTime">The base exposure time. Compared to UGC1</param>
	/// <param name="cycleCount">The amount of exposure cycles to make</param>
	/// <returns>The exposure times in seconds</returns>
	public int CalculateExposure(double adjustmentAngle, double baseTime = BASE_TIME_UGC2, int cycleCount = 1)
	{
		double slewTime = SlewFunction(adjustmentAngle);
		double waitTime = slewTime / 1.5;
		double exposureTime = ExposureTime(baseTime);
		double exposureTimeCheck = exposureTime / 2;

		return cycleCount * (int)Math.Ceiling(slewTime + waitTime + exposureTime + READOUT_TIME + exposureTimeCheck + READOUT_TIME);
	}

	public Point Point =>
		AzimuthalCoordinate is null ? new(-1, -1) : new(AzimuthalCoordinate!.Value.Azimuth, AzimuthalCoordinate!.Value.Height);
}