namespace Galaxylist.Models;

public class Viewport : IRatableObject
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
		(double deltaHeight, double deltaAzimuth) = (Pos.Declination.ToDegrees() - other.Pos.Declination.ToDegrees(),
			Pos.RightAscention.ToDegree() - other.Pos.RightAscention.ToDegree());

		return Math.Acos(Math.Cos(deltaAzimuth) * Math.Cos(deltaHeight));
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

	public double Quality() => Galaxies.Sum(galaxy => galaxy.Quality);

	public double DistanceBetween(IRatableObject other) => DistanceTo((Viewport)other);

	public AzimuthalCoordinate Position() => AzimuthalPos;

	public void Visit() =>
		Galaxies.ToList()
				.ForEach(x => x.Visit());

	public void Reset() =>
		Galaxies.ToList()
				.ForEach(x => x.Reset());

	public double WaitTime(double distance) => Galaxies.Max(x => x.CalculateExposure(distance));

	public double Exposure() => Galaxies.Max(x => x.Exposure());
}