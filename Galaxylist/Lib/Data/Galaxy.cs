namespace Galaxylist.Lib.Data;

/// <summary>
/// Data model of a single galaxy.
/// </summary>
public class Galaxy
{
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
		double distance = Distance;
		int inclination = Inclination;
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
		double resQuality = (int)(quality * 100) / 100d;

		return resQuality * Convert.ToInt32(!_visited);
	}

	private double ExposureTime(double baseTime) => baseTime * Math.Pow(91 / Distance, 2);

	private static double SlewFunction(double distance) => 1 / 3d * distance + 6;

	private const int READOUT_TIME = 3;

	/// <summary>
	/// Calculates the exposure time for a given adjustment angle in degrees.
	/// </summary>
	/// <param name="adjustmentAngle">The angle of the slew movement</param>
	/// <param name="baseTime">The base exposure time. Compared to UGC1</param>
	/// <param name="cycles">The amount of exposure cycles to make</param>
	/// <returns>The exposure times in seconds</returns>
	public int CalculateExposure(double adjustmentAngle, double baseTime = 60, int cycles = 1)
	{
		double slewTime = SlewFunction(adjustmentAngle);
		double waitTime = slewTime / 1.5;
		double exposureTime = ExposureTime(baseTime);
		double exposureTimeCheck = exposureTime / 2;

		return cycles * (int)Math.Ceiling(slewTime + waitTime + exposureTime + READOUT_TIME + exposureTimeCheck + READOUT_TIME);
	}
}