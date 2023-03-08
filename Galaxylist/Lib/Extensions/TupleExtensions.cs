namespace Galaxylist.Lib.Extensions;

public static partial class Extensions
{
	/// <summary>
	/// Converts a tuple to a <see cref="KeyValuePair{TKey,TValue}"/> of the same types.
	/// </summary>
	/// <param name="tuple">The tuple to convert</param>
	/// <typeparam name="TKey">The key type of the tuple</typeparam>
	/// <typeparam name="TValue">The value type of the tuple</typeparam>
	/// <returns>A new <see cref="KeyValuePair{TKey,TValue}"/> with the same values</returns>
	public static KeyValuePair<TKey, TValue> ToKvPair<TKey, TValue>(this (TKey, TValue) tuple) => new(tuple.Item1, tuple.Item2);
}