namespace Galaxylist.Lib.Filter.Absolute;

/// <summary>
/// Interface for filters to use in a <see cref="FilterPipeline{T}"/>
/// </summary>
/// <typeparam name="T">The type of items to filter</typeparam>
public interface IFilter<in T>
{
	/// <summary>
	/// Function which returns the filter function to use in the pipeline.
	/// </summary>
	/// <returns>The filter function to use in the pipeline</returns>
	public Func<T, bool> Func();
}