namespace Galaxylist.Filter;

public interface IRatableObject
{
	public double Quality();
	public double DistanceBetween(IRatableObject other);
	public AzimuthalCoordinate Position();
	public void Visit();
	
	public void Reset();

	public double WaitTime(double distance);

	public double Exposure();

	public DateTime? At();

	public DateTime? MarkAt(DateTime timestamp);
}