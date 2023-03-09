namespace Galaxylist.Filter;

public static class SituationalFilter
{
	public static IEnumerable<Galaxy> Filter(
		IEnumerable<Galaxy> galaxies,
		string hemisphere,
		double minHeight = 30,
		double maxMajorAxis = 10,
		double maxMinorAxis = 10
	)
	{
		IEnumerable<Galaxy> ret = galaxies.Where(galaxy =>
			{
				if (galaxy.AzimuthalCoordinate is null)
				{
					throw new ArgumentNullException(nameof(galaxy.AzimuthalCoordinate));
				}

				AzimuthalCoordinate coordinate = galaxy.AzimuthalCoordinate.Value;

				if (coordinate.Height < minHeight)
				{
					return false;
				}

				if (galaxy.SemiMajorAxis > maxMajorAxis || galaxy.SemiMinorAxis > maxMinorAxis)
				{
					return false;
				}

				return hemisphere switch
				{
					"E"   => coordinate.Azimuth is >= 0 and <= 180,
					"W"   => coordinate.Azimuth is >= 180 and <= 360,
					var _ => throw new ArgumentException($"Invalid hemisphere value: {hemisphere}", nameof(hemisphere))
				};
			}
		);

		return ret;
	}
}