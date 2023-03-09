namespace Galaxylist.Models;

/// <summary>
/// Class holding the neccessary data to calculate the ideal path of galaxies to take.
/// </summary>
public class CalculationData : BaseUserData
{
	public bool SendViewports { get; set; } = false;

	public double RasterApprox { get; set; } = 0.5;

	public double SearchRadius { get; set; } = 20;
	public int MaxSearchSeconds { get; set; } = 4 * 60 * 60;
	public int StartPointCount { get; set; } = 10;
}