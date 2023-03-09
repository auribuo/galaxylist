namespace Galaxylist.Api.Processors;

using FluentValidation.Results;
using Microsoft.Extensions.Logging;

/// <summary>
/// Postprocessor that logs the duration of the request.
/// </summary>
public class TimePostProcessor : IGlobalPostProcessor
{
	/// <summary>
	/// Logs the duration of the request.
	/// </summary>
	/// <param name="req">Request dto</param>
	/// <param name="res">Response dto</param>
	/// <param name="ctx">Http context</param>
	/// <param name="failures">Validation failures</param>
	/// <param name="ct">Cancellation token</param>
	/// <returns></returns>
	public Task PostProcessAsync(
		object req,
		object? res,
		HttpContext ctx,
		IReadOnlyCollection<ValidationFailure> failures,
		CancellationToken ct
	)
	{
		ctx.Resolve<ILogger<DurationLogger>>()
		   .LogInformation("Request took {0}ms", TimeState.ElapsedMilliseconds);

		return Task.CompletedTask;
	}
}