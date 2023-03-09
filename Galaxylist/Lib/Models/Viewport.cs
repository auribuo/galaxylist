namespace Galaxylist.Lib.Models;

public class Viewport
{
	public EquatorialCoordinate Pos { get; set; }
	public HashSet<Galaxy> Galaxies { get; } = new();
	
	public AzimuthalCoordinate TopLeft { get; set; }
	public AzimuthalCoordinate TopRight { get; set; }
	public AzimuthalCoordinate BottomLeft { get; set; }
	public AzimuthalCoordinate BottomRight { get; set; }
	
	
	public AzimuthalCoordinate AzimuthalPos { get; set; }

	public double DistanceTo(Viewport other)
	{
		(double deltaHeight, double deltaAzimuth) = (Pos.Declination.ToDegrees() - other.Pos.Declination.ToDegrees(), Pos.RightAscention.ToDegree() - other.Pos.RightAscention.ToDegree());

		return Math.Sqrt(deltaHeight * deltaHeight + deltaAzimuth * deltaAzimuth);
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