namespace Galaxylist.Filter;

using Data.Repo;

public class GalaxyRepo : IObjectRepo<Galaxy>
{
	public IEnumerable<Galaxy> Fetch() => GalaxyDataRepo.Galaxies();
}