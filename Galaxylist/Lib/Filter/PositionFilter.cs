namespace Galaxylist.Lib.Filter;

using Data;

public class PositionFilter : IFilter
{
	// TODO
	public bool Filter(Galaxy input) => input.EquatorialCoordinate.Declination.Minutes > 0;
}