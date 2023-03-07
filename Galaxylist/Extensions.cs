namespace Galaxylist;

public static class Extensions
{
	public static IEnumerable<T> SkipOut<T>(this IEnumerable<T> source, int count, out T item)
	{
		List<T> dup = source.ToList();
		item = dup.First();

		return dup.Skip(count);
	}

	public static IEnumerable<string?> SliceAccordingTo(this string s, IEnumerable<int> sliceLengths)
	{
		int start = 0;
		sliceLengths = sliceLengths.ToList();
		List<string>? res = new List<string?>();

		foreach (int length in sliceLengths)
		{
			if (start + length > s.Length)
			{
				res.Add(null);

				continue;
			}

			string token = s.Substring(start, length);
			res.Add(string.IsNullOrWhiteSpace(token) ? null : token);
			start += length + 1 + Convert.ToInt32(sliceLengths.Last() == length);
		}

		return res;
	}
}