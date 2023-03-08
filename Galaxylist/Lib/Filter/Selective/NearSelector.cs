namespace Galaxylist.Lib.Filter.Selective;

using Data.Repo;

public class NearSelector
{
	private readonly IGalaxyDataRepo _repo;
	private readonly CalculationData _data;
	private readonly int _maxViewports;

	public NearSelector(IGalaxyDataRepo repo, CalculationData data, int maxViewports = 100)
	{
		_repo = repo;
		_data = data;
		_maxViewports = maxViewports;
	}

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

		FilterPipeline<Galaxy> filter =
			FilterRepo.Default(_data.Hemisphere, _data.MinimumHeight, _data.MaxSemiMajorAxis, _data.MaxSemiMinorAxis);

		newGalaxies = filter.Filter(newGalaxies);
		List<Viewport> newViewports = newGalaxies.ToList()
												 .CalculateViewports(_data.Fov);

		NextViewport(next, newViewports, results, collected + 1);
	}
}