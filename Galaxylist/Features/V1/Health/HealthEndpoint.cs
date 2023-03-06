namespace Galaxylist.Features.V1.Health;

public class HealthEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
	public override void Configure()
	{
		Get("/health");
		Version(1);
		AllowAnonymous();
	}

	public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
	{
		await SendOkAsync();
	}
}