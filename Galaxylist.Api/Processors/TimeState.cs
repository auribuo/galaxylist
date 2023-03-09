namespace Galaxylist.Api.Processors;

using System.Diagnostics;

/// <summary>
/// A static class that holds the stopwatch for the processors.
/// </summary>
public static class TimeState
{
	private static readonly Stopwatch Stopwatch = new();

	/// <summary>
	/// The elapsed milliseconds since the stopwatch was started.
	/// </summary>
	public static long ElapsedMilliseconds
	{
		get
		{
			Stopwatch.Stop();
			long res = Stopwatch.ElapsedMilliseconds;
			Stopwatch.Reset();

			return res;
		}
	}

	/// <summary>
	/// Starts the stopwatch.
	/// </summary>
	public static void Start() => Stopwatch.Start();
}