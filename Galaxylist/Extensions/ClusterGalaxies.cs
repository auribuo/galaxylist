namespace Galaxylist.Extensions;

using Dbscan;
using Models;

public static partial class Extensions
{

    public static ClusterSet<Galaxy> Cluster(this List<Galaxy> galaxies, double epsilon, int minimumPointsPerCluster)
    {
        return global::Dbscan.Dbscan.CalculateClusters(galaxies, epsilon, minimumPointsPerCluster);
    }
}