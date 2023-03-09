namespace Galaxylist.Api.Processors;

using FluentValidation.Results;

/// <summary>
/// Preprocessor that starts the stopwatch.
/// </summary>
public class TimePreProcessor : IGlobalPreProcessor
{
	/// <summary>
	/// Starts the stopwatch.
	/// </summary>
	/// <param name="req">Request dto</param>
	/// <param name="ctx">Http context</param>
	/// <param name="failures">Validation failures</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns></returns>
	public Task PreProcessAsync(object req, HttpContext ctx, List<ValidationFailure> failures, CancellationToken ct)
	{
		TimeState.Start();

		return Task.CompletedTask;
	}
}