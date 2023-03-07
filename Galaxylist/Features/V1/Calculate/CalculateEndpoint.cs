namespace Galaxylist.Features.V1.Calculate;

using Lib;

public class CalculateEndpoint : Endpoint<CalculateRequest, CalculateResponse>
{
	private readonly IGalaxyCalculator _galaxyCalculator;

	public CalculateEndpoint(IGalaxyCalculator galaxyCalculator)
	{
		_galaxyCalculator = galaxyCalculator;
	}

	public override void Configure()
	{
		Post("/calculate");
		Version(1);
		AllowAnonymous();
	}

	public override async Task HandleAsync(CalculateRequest req, CancellationToken ct)
	{
		CalculateResponse response = _galaxyCalculator.Calculate(req);
		await SendAsync(response);
	}
}