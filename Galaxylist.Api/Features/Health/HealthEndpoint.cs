namespace Galaxylist.Api.Features.V1.Health;

using FastEndpoints;
using Microsoft.AspNetCore.Http;

/// <summary>
///  Endpoint that returns a 200 OK response if the server is running.
/// </summary>
public class HealthEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
	/// <summary>
	/// <inheritdoc cref="BaseEndpoint.Configure"/>
	/// </summary>
	public override void Configure()
	{
		Get("/health");
		Description(endpoint =>
			{
				endpoint.Produces(200);
			}
		);

		AllowAnonymous();
	}

	/// <summary>
	/// <inheritdoc cref="FastEndpoints.Endpoint{TRequest,TResponse}.HandleAsync"/>
	/// </summary>
	/// <param name="req">Request dto</param>
	/// <param name="ct">Cancellation token</param>
	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		await SendOkAsync();
	}
}