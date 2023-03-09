namespace Galaxylist.Lib.Extensions;

/// <summary>
/// Class holding all extension methods
/// </summary>
public static partial class Extensions
{
	/// <summary>
	/// Skips <paramref name="count"/> elements in the given collection and returns the skipped elements in an array.
	/// </summary>
	/// <param name="source">The source enumerable</param>
	/// <param name="count">The amount of items to skip</param>
	/// <param name="item">The skipped items</param>
	/// <typeparam name="T">The type of the elements in source</typeparam>
	/// <returns>An <see cref="IEnumerable{T}"/> with all the items after the skipped sequence</returns>
	/// <exception cref="ArgumentException">Thrown if count is less or equal to 0</exception>
	public static IEnumerable<T> SkipOut<T>(this IEnumerable<T> source, int count, out T[] item)
	{
		if (count <= 0)
		{
			throw new ArgumentException("Count must be greater than 0.", nameof(count));
		}

		T[] dup = source.ToArray();
		item = dup[..count];

		return dup.Skip(count);
	}

	/// <summary>
	/// Slices the given string accordin to the given slice lengths in <paramref name="sliceLengths"/>.
	/// If the string is too short to slice according to the given lengths, the remaining slices will be null.
	/// After each slice a character is skipped, assuming the parts to slice are separated by a character.
	/// </summary>
	/// <param name="s">The string to slice</param>
	/// <param name="sliceLengths">An enumerable holding the length of each slice</param>
	/// <returns>The sliced string</returns>
	public static IEnumerable<string?> SliceAccordingTo(this string s, IEnumerable<int> sliceLengths)
	{
		int start = 0;
		sliceLengths = sliceLengths.ToList();
		List<string?> res = new();

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