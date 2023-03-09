namespace Galaxylist.Models;

public class Viewport
{
	public AzimuthalCoordinate Pos { get; set; }
	public HashSet<Galaxy> Galaxies { get; } = new();

	public double DistanceTo(Viewport other)
	{
		(double deltaHeight, double deltaAzimuth) = (Pos.Height - other.Pos.Height, Pos.Azimuth - other.Pos.Azimuth);

		return Math.Acos(Math.Cos(deltaAzimuth)*Math.Cos(deltaHeight));
	}

	private bool _isVisited;

	public bool IsVisited
	{
		get => _isVisited;
		set
		{
			if (value)
			{
				Galaxies.ToList()
						.ForEach(galaxy => galaxy.Visit());
			}
			else
			{
				Galaxies.ToList()
						.ForEach(galaxy => galaxy.Reset());
			}

			_isVisited = value;
		}
	}
}