namespace Galaxylist.Features.V1.Calculate;

using Lib.Models;

public class CalculateRequest
{
	public required DateTime ObservationStart { get; set; }
	public required Location Location { get; set; }
	public required Telescope Telescope { get; set; }
	public required Fov Fov { get; set; }
}