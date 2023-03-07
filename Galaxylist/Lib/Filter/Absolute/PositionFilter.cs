namespace Galaxylist.Lib.Filter.Absolute;

/// <summary>
/// Filter wich filters the galaxies based on their azimuthal height.
/// </summary>
public class PositionFilter : IFilter<Galaxy>
{
	private readonly int _minHeight;

	/// <summary>
	/// Creates a new instance of the filter which filters the galaxies based on their azimuthal height.
	/// It excludes all galaxies with an azimuthal height lower than the given minimum height.
	/// </summary>
	/// <param name="minHeight"></param>
	public PositionFilter(int minHeight)
	{
		_minHeight = minHeight;
	}

	/// <summary>
	/// Returns the filter function to use in the pipeline.
	/// The filter function filters the galaxies based on their azimuthal height and excludes all galaxies below the given height.
	/// </summary>
	/// <returns>The filter function to add to the pipeline</returns>
	public Func<Galaxy, bool> Func() => input => input.AzimuthalCoordinate!.Value.Height > _minHeight;
}