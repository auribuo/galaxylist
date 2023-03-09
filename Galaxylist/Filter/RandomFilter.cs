namespace Galaxylist.Filter;

public class RandomFilter
{
	public static IEnumerable<T> Filter<T>(IEnumerable<T> objects, int maxSearchSeconds = 5 * 60 * 60) where T : IRatableObject
	{
		var random = new Random();
		var objectsList = objects.ToList();
		var path = new List<T>();
		PropagatePath(maxSearchSeconds, random, objectsList, path);

		return path;
	}

	private static void PropagatePath<T>(int maxSearchSeconds, Random random, List<T> objectsList, List<T> path, int elapsedSeconds = 0)
		where T : IRatableObject
	{
		while (true)
		{
			if (elapsedSeconds >= maxSearchSeconds)
			{
				return;
			}

			var next = objectsList[random.Next(objectsList.Count)];
			path.Add(next);
			objectsList.Remove(next);
			var waitTime = next.WaitTime(0);
			elapsedSeconds += (int)waitTime;
		}
	}
}