namespace Galaxylist.Lib.Filter.Absolute;

/// <summary>
/// Filter pipeline to filter a list of items through multiple filters.
/// </summary>
/// <typeparam name="T">The type of items to filter</typeparam>
public class FilterPipeline<T>
{
	/// <summary>
	/// Constructor is private to prevent instantiation via the constructor.
	/// </summary>
	private FilterPipeline() { }

	/// <summary>
	/// Creates a new instance of the <see cref="FilterPipeline{T}"/> class.
	/// </summary>
	/// <returns>A new instance of the <see cref="FilterPipeline{T}"/> class.</returns>
	public static FilterPipeline<T> New() => new();

	// list of filters
	private readonly List<Func<T, bool>> _filters = new();

	/// <summary>
	/// Adds a new filter function to the pipeline.
	/// </summary>
	/// <param name="func">The filter function to add to the pipeline</param>
	/// <returns>The current instance of the pipeline</returns>
	public FilterPipeline<T> Add(Func<T, bool> func)
	{
		_filters.Add(func);

		return this;
	}

	/// <summary>
	/// Adds a new <see cref="IFilter{T}"/> to the pipeline and registers the filter function via <see cref="IFilter{T}.Func"/>.
	/// </summary>
	/// <param name="filter">The filter to add to the pipeline</param>
	/// <returns>The current instance of the pipeline</returns>
	public FilterPipeline<T> With(IFilter<T> filter)
	{
		_filters.Add(filter.Func());

		return this;
	}

	/// <summary>
	/// Filters a given sequence of items filtered using the pipeline.
	/// </summary>
	/// <param name="items">The items to filter</param>
	/// <returns>The filtered items</returns>
	public IEnumerable<T> Filter(IEnumerable<T> items) => items.Where(item => _filters.All(f => f(item)));
}