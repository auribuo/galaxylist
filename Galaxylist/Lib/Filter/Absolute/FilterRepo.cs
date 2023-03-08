namespace Galaxylist.Lib.Filter.Absolute;

public static class FilterRepo
{
	public static FilterPipeline<Galaxy> Default(string meridian, int minHeight = 30, double maxMajorAxis = 10, double maxMinorAxis = 30) =>
		FilterPipeline<Galaxy>.New()
							  .With(new PositionFilter(minHeight))
							  .With(new MeridianFilter(meridian))
							  .With(new SizeFilter(maxMajorAxis, maxMinorAxis));
}