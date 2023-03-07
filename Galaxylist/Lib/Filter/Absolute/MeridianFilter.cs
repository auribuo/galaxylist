namespace Galaxylist.Lib.Filter.Absolute;

/// <summary>
/// Filter which filters the galaxies by on wich side of the meridian based on the observer they are.
/// </summary>
public class MeridianFilter : IFilter<Galaxy>
{
	private readonly string _hemisphere;

	/// <summary>
	/// Creates a new instace of the filter with the given hemisphere side. The hemisphere side can be either "E" or "W".
	/// </summary>
	/// <param name="hemisphere"></param>
	public MeridianFilter(string hemisphere)
	{
		_hemisphere = hemisphere;
	}

	/// <summary>
	/// Returns the filter function to use in the pipeline.
	/// The filter function filters the galaxies by on wich side of the meridian based on the observer they are using the azimuth element of the <see cref="Galaxy.AzimuthalCoordinate"/> coordinates.
	/// </summary>
	/// <returns>The filter function to add to the pipeline</returns>
	/// <exception cref="ArgumentException">Thrown if the provided hemisphere</exception>
	public Func<Galaxy, bool> Func() =>
		input =>
		{
			return _hemisphere switch
			{
				"E"   => input.AzimuthalCoordinate!.Value.Azimuth is > 0 and < 180,
				"W"   => input.AzimuthalCoordinate!.Value.Azimuth is > 180 and < 360,
				var _ => throw new ArgumentException("Invalid hemisphere.", nameof(_hemisphere)),
			};
		};
}