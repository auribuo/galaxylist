namespace Galaxylist.Lib.Models;

public class Viewport
{
    public AzimuthalCoordinate pos { get; set; }= new AzimuthalCoordinate();
    public HashSet<Galaxy> Galaxies { get; } = new HashSet<Galaxy>();
}