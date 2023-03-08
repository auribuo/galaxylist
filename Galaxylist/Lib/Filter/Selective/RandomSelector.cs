namespace Galaxylist.Lib.Filter.Selective;

/// <summary>
/// A selector that selects a random number of items from a list of <see cref="Viewport"/>
/// </summary>
public class RandomSelector
{
	private readonly int _maxViewportCount;

	/// <summary>
	/// Creates a new random selector that selects a maximum of <paramref name="maxViewportCount"/> items.
	/// </summary>
	/// <param name="maxViewportCount">Maximum items to select</param>
	public RandomSelector(int maxViewportCount = 100)
	{
		_maxViewportCount = maxViewportCount;
	}

	/// <summary>
	/// Selects viewports by randomly selecting them from the list of viewports.
	/// </summary>
	/// <param name="items">The list of viewports to select</param>
	/// <returns>The randomly selected viewports</returns>
	public IEnumerable<Viewport> Select(IEnumerable<Viewport> items)
	{
		Random random = new();
		List<Viewport> result = new();
		var itemList = items.ToList();
		int count = Math.Min(itemList.Count, _maxViewportCount);

		for (int i = 0; i < count; i++)
		{
			int index = random.Next(itemList.Count);
			result.Add(itemList[index]);
			itemList.RemoveAt(index);
		}

		return result;
	}
}