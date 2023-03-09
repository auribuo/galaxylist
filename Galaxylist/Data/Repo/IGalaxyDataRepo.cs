namespace Galaxylist.Data.Repo;

using Models;

/// <summary>
/// Interface for a repository of galaxies.
/// </summary>
public interface IGalaxyDataRepo
{
	/// <summary>
	/// Returns all galaxies in the repository.
	/// </summary>
	/// <returns>Collection of <see cref="Galaxy"/></returns>
	public IEnumerable<Galaxy> Galaxies();
}