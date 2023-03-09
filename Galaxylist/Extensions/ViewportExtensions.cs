namespace Galaxylist.Extensions;

using Models;

public static partial class Extensions
{
	private static double GetNearestDeg(double degree, double step)
	{
		double nearestDeg;

		//Console.WriteLine("Step: "+step);
		//Console.WriteLine("Degree: "+degree);
		int n = (int)(degree / step);

		//Console.WriteLine("N: "+n);

		if (double.Abs(degree - n * step) < double.Abs(degree - (n + 1) * step))
		{
			nearestDeg = n * step;
		}
		else
		{
			nearestDeg = (n + 1) * step;
		}

		//Console.WriteLine("Nearest: "+nearestDeg);
		return nearestDeg;
	}

	private static bool IsInViewport(AzimuthalCoordinate pos, Fov fov, AzimuthalCoordinate viewportPos) =>
		viewportPos.Azimuth - fov.Width / 2 < pos.Azimuth && viewportPos.Azimuth + fov.Width / 2 > pos.Azimuth &&
		viewportPos.Height - fov.Height / 2 < pos.Height && viewportPos.Height + fov.Height / 2 > pos.Height;

	/// <summary>
	/// Calculates interesting viewports with galaxies in them. Viewports are approximated to the nearest number at  x * fov.Height * rasterApprox and x* fov.Width *rasterApprox
	/// </summary>
	/// <param name="galaxies"></param>
	/// <param name="fov">Viewport of the camera on the telescope</param>
	/// <param name="rasterApprox">Approximation factor which represents the fraction in which the fov position is approximated relative to the size of the fov.</param>
	/// <returns>A list of viewports. One galaxy can be in multiple viewports</returns>
	public static List<Viewport> CalculateViewports(this List<Galaxy> galaxies, Fov fov, double rasterApprox = 0.25)
	{
		Dictionary<(double, double), Viewport> viewports = new();
		double xStep = fov.Width * rasterApprox;
		double yStep = fov.Height * rasterApprox;
		int nXStep = (int)(fov.Width / xStep) / 2;
		int nYStep = (int)(fov.Height / yStep) / 2;
		Console.WriteLine(fov.Height);
		Console.WriteLine(fov.Width);
		Console.WriteLine(xStep);
		Console.WriteLine(yStep);
	
		foreach (Galaxy galaxy in galaxies)
		{
			double xApprox = GetNearestDeg(galaxy.AzimuthalCoordinate!.Value.Azimuth, xStep);
			double yApprox = GetNearestDeg(galaxy.AzimuthalCoordinate.Value.Height, yStep);

			try
			{
				Viewport viewport = viewports[(xApprox, yApprox)];
				viewport.Galaxies.Add(galaxy);
			}
			catch (Exception)
			{
				Viewport viewport = new()
				{
					Pos = (xApprox, yApprox),
				};

				viewport.Galaxies.Add(galaxy);
				viewports[(xApprox, yApprox)] = viewport;
			}

			for (double ySearch = yApprox - nYStep * yStep; ySearch < yApprox + nYStep * yStep; ySearch += yStep)
			{
				for (double xSearch = xApprox - nXStep * xStep; xSearch < xApprox + nXStep * xStep; xSearch += xStep)
				{
					if (!IsInViewport(galaxy.AzimuthalCoordinate.Value, fov, (xSearch, ySearch)) ||
					    (Math.Abs(xSearch - xApprox) > 0.000002 && Math.Abs(ySearch - yApprox) > 0.000002))
					{
						continue;
					}

					try
					{
						Viewport viewport = viewports[(xSearch, ySearch)];
						viewport.Galaxies.Add(galaxy);
					}
					catch (Exception)
					{
						Viewport viewport = new()
						{
							Pos = (xApprox, yApprox),
						};

						viewport.Galaxies.Add(galaxy);
						viewports[(xSearch, ySearch)] = viewport;
					}
				}
			}
		}

		foreach (Galaxy galaxy in galaxies)
		{
			double xApprox = GetNearestDeg(galaxy.AzimuthalCoordinate!.Value.Azimuth, xStep);
			double yApprox = GetNearestDeg(galaxy.AzimuthalCoordinate.Value.Height, yStep);

			for (double ySearch = yApprox - nYStep * yStep; ySearch < yApprox + nYStep * yStep; ySearch += yStep)
			{
				for (double xSearch = xApprox - nXStep * xStep; xSearch < xApprox + nXStep * xStep; xSearch += xStep)
				{
					if (!IsInViewport(galaxy.AzimuthalCoordinate.Value, fov, (xSearch, ySearch)) ||
					    (Math.Abs(xSearch - xApprox) > 0.000002 && Math.Abs(ySearch - yApprox) > 0.000002))
					{
						continue;
					}

					try
					{
						if (viewports[(xSearch, ySearch)]
						    .Galaxies.Count <= 1)
						{
							viewports.Remove((xSearch, ySearch));
						}
					}
					catch (Exception)
					{
						// ignored
					}
				}
			}
		}

		return new List<Viewport>(viewports.Values);
	}

	/// <summary>
	/// Calculate the quality of a viewport by summing the quality of all galaxies in it
	/// </summary>
	/// <param name="viewport">The viewport to calculate the quality of</param>
	/// <returns>The total quality of a viewport</returns>
	public static double CalculateQuality(this Viewport viewport)
	{
		return viewport.Galaxies.Sum(galaxy => galaxy.Quality);
	}

	/// <summary>
	/// Calculate the quality of a viewport by summing the quality of all galaxies in it.
	/// Sets the visited flag of all galaxies in the viewport to true.
	/// </summary>
	/// <param name="viewport">The viewport to calculate the quality of</param>
	/// <returns>The total quality of a viewport</returns>
	public static double CalculateQualityAndDiscard(this Viewport viewport)
	{
		double quality = viewport.Galaxies.Sum(galaxy => galaxy.Quality);

		foreach (Galaxy galaxy in viewport.Galaxies)
		{
			galaxy.Visit();
		}
		return quality;
	}
}