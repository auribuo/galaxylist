namespace Galaxylist.Api.Features.Calculate;

using Filter;
using Models;

/// <summary>
/// Class that represents the response of the <see cref="CalculateEndpoint"/> endpoint. It contains the total number of galaxies and the list of galaxies calculated.
/// </summary>
public class CalculateResponse
{
	/// <summary>
	/// Total number of galaxies in the result
	/// </summary>
	public required int Total { get; set; }

	public required double TotalQuality { get; set; }

	public required double TotalExposure { get; set; }
	
	public required double ExposureDeviation { get; set; }
	
	public required double MaxSearchSeconds { get; set; }

	/// <summary>
	/// The list of galaxies calculated
	/// </summary>
	public required List<Galaxy> GalaxyPath { get; set; }

	public required List<Viewport> GalaxyPathViewports { get; set; }
	public required List<Viewport> ViewportPath { get; set; }
}