namespace Galaxylist.Lib.Filter.Absolute;

/// <summary>
/// Filter which filters the galaxies by their size in arcminutes.
/// </summary>
public class SizeFilter : IFilter<Galaxy>
{
	private readonly double _maxMajorAxis;
	private readonly double _maxMinorAxis;

	/// <summary>
	/// Creates a new instance of the filter with the given maximum major and maxor axis.
	/// </summary>
	/// <param name="maxMajorAxis">The optional maximum major axis</param>
	/// <param name="maxMinorAxis">The optional maximum maxor axis</param>
	public SizeFilter(double maxMajorAxis = 10, double maxMinorAxis = 10)
	{
		_maxMajorAxis = maxMajorAxis;
		_maxMinorAxis = maxMinorAxis;
	}

	/// <summary>
	/// Returns the filter function to use in the pipeline.
	/// The filter function filters the galaxies by their size in arcminutes.
	/// </summary>
	/// <returns>The filter function to use in the pipeline</returns>
	public Func<Galaxy, bool> Func() => galaxy => galaxy.SemiMajorAxis < _maxMajorAxis && galaxy.SemiMinorAxis < _maxMinorAxis;
}