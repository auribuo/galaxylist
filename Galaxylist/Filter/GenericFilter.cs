namespace Galaxylist.Filter;

public class GenericFilter
{
	public static IEnumerable<T> Filter<T>(
		IEnumerable<T> objects,
		IObjectRepo<T> repo,
		DateTime startTime,
		double searchRadius,
		int startPointCount = 10,
		int maxSearchSeconds = 5 * 60 * 60
	) where T : IRatableObject
	{
		var objectsList = objects.ToList();
		var startPoints = objectsList.OrderByDescending(obj => obj.Quality())
									 .Take(Math.Min(objectsList.Count, startPointCount));

		var paths = new List<List<T>>();
		var tasks = new List<Task>();

		foreach (T startPoint in startPoints)
		{
			var task = Task.Run(() => CalculatePath(repo,
													startTime,
													searchRadius,
													maxSearchSeconds,
													startPoint,
													objectsList,
													paths
								)
			);

			tasks.Add(task);
		}

		Task.WaitAll(tasks.ToArray());
		var bestPath = paths.OrderByDescending(x => x.Sum(y => y.Quality()))
							.First();

		// check for duplicates
		var duplicates = bestPath.GroupBy(x => x)
								 .Where(g => g.Count() > 1)
								 .Select(g => g.Key)
								 .ToList();

		if (duplicates.Any())
		{
			throw new Exception($"Duplicate objects in path: {string.Join(", ", duplicates)}");
		}

		bestPath.ForEach(x => x.Reset());

		return bestPath;
	}

	private static Task CalculatePath<T>(
		IObjectRepo<T> repo,
		DateTime startTime,
		double searchRadius,
		int maxSearchSeconds,
		T startPoint,
		List<T> objectsList,
		List<List<T>> paths
	) where T : IRatableObject
	{
		List<T> path = new()
		{
			startPoint,
		};

		int elapsedSeconds = 0;
		Propagate(objectsList,
				  repo,
				  startPoint,
				  searchRadius,
				  startTime,
				  path,
				  maxSearchSeconds,
				  elapsedSeconds
		);

		paths.Add(path);

		return Task.CompletedTask;
	}

	private static void Propagate<T>(
		IEnumerable<T> objects,
		IObjectRepo<T> repo,
		IRatableObject startPoint,
		double searchRadius,
		DateTime time,
		List<T> path,
		int maxSearchSeconds,
		int elapsedSeconds
	) where T : IRatableObject
	{
		if (elapsedSeconds >= maxSearchSeconds)
		{
			return;
		}

		T next;

		if (Math.Abs(searchRadius - -1) < 10e-6)
		{
			Console.WriteLine("Skipping search radius");
			next = objects.Where(obj => !path.Contains(obj))
						  .MaxBy(obj => obj.Quality())!;
		}
		else
		{
			next = objects.Where(obj => obj.DistanceBetween(startPoint) <= searchRadius)
						  .Where(obj => !path.Contains(obj))
						  .MaxBy(obj => obj.Quality())!;
		}

		var distance = startPoint.DistanceBetween(next);
		var waitSec = startPoint.WaitTime(distance);
		elapsedSeconds += (int)waitSec;
		var nextTime = time.AddSeconds(waitSec);
		var newObjects = repo.Fetch()
							 .ToList();

		newObjects.Where(path.Contains)
				  .ToList()
				  .ForEach(x => x.Visit());

		path.Add(next);
		Propagate(newObjects,
				  repo,
				  next,
				  searchRadius,
				  nextTime,
				  path,
				  maxSearchSeconds,
				  elapsedSeconds
		);
	}
}