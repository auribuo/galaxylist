namespace Galaxylist.Lib.Workbook;

public static  class CalculateNeighbours
{
    public static void calc()
    {
        
        UgcDataRepo repo = UgcDataRepo.New();
        List<Galaxy> galaxies = repo.Galaxies.ToList();


        Dictionary<Galaxy, List<Galaxy>> neighbours = new Dictionary<Galaxy, List<Galaxy>>();

        double maxDist = 0.3;
		
        Console.WriteLine(galaxies.Count);
        foreach (var galaxy in galaxies)
        {
			
            double Gal1Declination = galaxy.EquatorialCoordinate.Declination.ToDegrees();
            double Gal1RightAscention = galaxy.EquatorialCoordinate.RightAscention.ToDegree();

			
            neighbours[galaxy] = new List<Galaxy>();
            foreach (var galaxy2 in galaxies){
				
				
                if (galaxy2 == galaxy)
                {
                    continue;
                }

                double gal2Declination = galaxy2.EquatorialCoordinate.Declination.ToDegrees();
                double gal2RightAscention = galaxy2.EquatorialCoordinate.RightAscention.ToDegree();

                double declinationDiff = gal2Declination - Gal1Declination;
                double rightAscentionDifF = gal2RightAscention - Gal1RightAscention;

                double dist = Math.Sqrt(Math.Pow(declinationDiff, 2) +
                                        Math.Pow(rightAscentionDifF, 2));
				
                if (dist < maxDist)
                {
                    neighbours[galaxy].Add(galaxy2);
                }
				
            }
        }
		
        Console.WriteLine(neighbours.Values.Count(galaxy => galaxy.Count ==4));
    }
}