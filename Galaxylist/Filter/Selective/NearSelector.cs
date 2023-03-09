namespace Galaxylist.Filter.Selective;

using Data.Repo;
using Extensions;
using Models;

/// <summary>
/// Selector which only prioritizes the nearest viewports.
/// </summary>
public class NearSelector
{
	private readonly IGalaxyDataRepo _repo;
	private readonly CalculationData _data;
	private readonly int _maxViewports;

	/// <summary>
	/// Creates a new instance of the NearSelector.
	/// </summary>
	/// <param name="repo">The <see cref="IGalaxyDataRepo"/> to fetch updated galaxy data</param>
	/// <param name="data">The calculation parameters</param>
	/// <param name="maxViewports">Max viewports to select</param>
	public NearSelector(IGalaxyDataRepo repo, CalculationData data, int maxViewports = 100)
	{
		_repo = repo;
		_data = data;
		_maxViewports = maxViewports;
	}

	/// <summary>
	/// Selects from the given viewports the best one then the nearest one to it and so on.
	/// </summary>
	/// <param name="items">The items to select</param>
	/// <returns>The selected viewports</returns>
	public IEnumerable<Viewport> Select(IEnumerable<Viewport> items)
	{
		List<Viewport> itemList = items.ToList();
		Dictionary<Viewport, double> viewportQualities = new();
		itemList.ForEach(viewport => viewportQualities.Add(viewport, viewport.CalculateQuality()));
		IEnumerable<Viewport> sortedViewports = viewportQualities.OrderByDescending(viewport => viewport.Value)
																 .Select(kv => kv.Key);

		List<Viewport> result = new();
		Viewport start = sortedViewports.First();
		result.Add(start);
		NextViewport(start, itemList, result);

		return result;
	}

	/// <summary>
	/// Calculates the next nearest viewport recursively.
	/// </summary>
	/// <param name="current">The viewport we are currently at</param>
	/// <param name="viewports">The current available viewports</param>
	/// <param name="results">The selected viewports</param>
	/// <param name="collected">How many viewports are currenly in the results</param>
	private void NextViewport(Viewport current, IList<Viewport> viewports, ICollection<Viewport> results, int collected = 1)
	{
		if (collected >= _maxViewports)
		{
			return;
		}

		Viewport next = viewports.Where(v => !v.IsVisited)
								 .Select(v => (viewport: v, distance: current.DistanceTo(v)))
								 .MinBy(x => x.distance)
								 .viewport;

		viewports[viewports.IndexOf(current)]
			.IsVisited = true;

		results.Add(next);
		int waitTime = next.Galaxies.Select(g => g.CalculateExposure(current.DistanceTo(next)))
						   .Max();

		_data.ObservationStart = _data.ObservationStart.AddSeconds(waitTime);
		IEnumerable<Galaxy> newGalaxies = _repo.Galaxies()
											   .Select(g =>
												   {
													   g.AzimuthalCoordinate =
														   g.EquatorialCoordinate.ToAzimuthal(_data.ObservationStart, _data.Location);

													   return g;
												   }
											   );

		newGalaxies = SituationalFilter.Filter(newGalaxies, _data.Hemisphere, _data.MinimumHeight, _data.MaxSemiMajorAxis, _data.MaxSemiMinorAxis);
		List<Viewport> newViewports = newGalaxies.ToList()
												 .CalculateViewports(_data.Fov);

		NextViewport(next, newViewports, results, collected + 1);
	}
}