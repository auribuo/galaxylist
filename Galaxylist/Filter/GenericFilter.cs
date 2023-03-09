namespace Galaxylist.Filter;

using Data.Repo;

public class GenericFilter
{
	public static IEnumerable<T> Filter<T>(IEnumerable<T> objects, CalculationData data) where T : class, IRatableObject
	{
		List<T> objectsList = objects.ToList();
		IEnumerable<T> startPoints = objectsList.OrderByDescending(obj => obj.Quality())
												.Take(Math.Min(objectsList.Count, data.StartPointCount));

		List<List<T>> paths = new();
		Task.WaitAll(startPoints.Select(startPoint => Task.Run(() =>
										    {
											    T[] arr = new T[objectsList.Count];
											    objectsList.CopyTo(arr);
											    CalculatePath(data, startPoint, arr.ToList(), paths);
										    }
									    )
							    )
							    .ToArray()
		);

		List<T> bestPath = paths.OrderByDescending(x => x.Sum(y => y.Quality()))
								.First();

		// check for duplicates
		List<T> duplicates = bestPath.GroupBy(x => x)
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

	private static void CalculatePath<T>(CalculationData data, T startPoint, IEnumerable<T> objectsList, ICollection<List<T>> paths)
		where T : class, IRatableObject
	{
		List<T> path = new()
		{
			startPoint,
		};

		int elapsedSeconds = (int)startPoint.WaitTime(0);
		Propagate(objectsList, startPoint, path, data, elapsedSeconds);
		paths.Add(path);
	}

	private static void Propagate<T>(
		IEnumerable<T> objects,
		IRatableObject startPoint,
		ICollection<T> path,
		CalculationData data,
		int elapsedSeconds
	) where T : class, IRatableObject
	{
		if (elapsedSeconds >= data.MaxSearchSeconds)
		{
			return;
		}

		T next;

		if (Math.Abs(data.SearchRadius - -1) < 10e-6)
		{
			Console.WriteLine("Skipping search radius");
			next = objects.Where(obj => !path.Contains(obj))
						  .MaxBy(obj => obj.Quality())!;
		}
		else
		{
			next = objects.Where(obj => obj.DistanceBetween(startPoint) <= data.SearchRadius)
						  .Where(obj => !path.Contains(obj))
						  .MaxBy(obj => obj.Quality())!;
		}

		double distance = startPoint.DistanceBetween(next);
		double waitSec = startPoint.WaitTime(distance);
		elapsedSeconds += (int)waitSec;
		DateTime nextTime = data.ObservationStart.AddSeconds(waitSec);
		IEnumerable<Galaxy> newObjects = GalaxyDataRepo.Galaxies();
		List<T> newObjectList;

		if (data.SendViewports)
		{
			newObjectList = newObjects.CalculateViewports(data.Fov, data.Location, data.ObservationStart)
									  .Cast<T>()
									  .ToList();
		}
		else
		{
			newObjectList = newObjects.Cast<T>()
									  .ToList();
		}

		data.ObservationStart = nextTime;
		newObjectList.Where(path.Contains)
					 .ToList()
					 .ForEach(x => x.Visit());

		path.Add(next);
		Propagate(newObjectList, next, path, data, elapsedSeconds);
	}
}