namespace Galaxylist.Lib;

using Features.V1.Calculate;

public interface IGalaxyCalculator
{
	public CalculateResponse Calculate(CalculateRequest request);
}