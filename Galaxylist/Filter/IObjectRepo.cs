namespace Galaxylist.Filter;

public interface IObjectRepo<out T>  where T : IRatableObject
{
	public IEnumerable<T> Fetch();
}