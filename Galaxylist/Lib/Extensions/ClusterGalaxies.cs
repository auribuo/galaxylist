using Dbscan;

namespace Galaxylist.Lib.Extensions;

public static partial class Extensions
{

    public static ClusterSet<Galaxy> Cluster(this List<Galaxy> galaxies, double epsilon, int minimumPointsPerCluster)
    {
        return Dbscan.Dbscan.CalculateClusters(galaxies, epsilon, minimumPointsPerCluster);
    }
}