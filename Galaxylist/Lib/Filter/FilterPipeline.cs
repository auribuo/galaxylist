namespace Galaxylist.Lib.Filter;

using Data;

public class FilterPipeline
{
	public static FilterPipeline New() => new();

	private readonly List<IFilter> _filters = new();

	public FilterPipeline Add(IFilter filter)
	{
		_filters.Add(filter);

		return this;
	}

	public IEnumerable<Galaxy> Filter(IEnumerable<Galaxy> galaxies) =>
		galaxies.Where(galaxy => _filters.All(filter => filter.Filter(galaxy)));
}