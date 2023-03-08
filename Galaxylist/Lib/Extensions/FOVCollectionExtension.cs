namespace Galaxylist.Lib.Extensions;

public static partial class Extensions
{

    private static double getNearestDeg(double degree, double step)
    {
        double nearestDeg = 0;
        
        //Console.WriteLine("Step: "+step);
        //Console.WriteLine("Degree: "+degree);
        
        int n = (int)(degree / step);
        
        //Console.WriteLine("N: "+n);

        if (Double.Abs(degree - n * step) < Double.Abs(degree - (n+1) * step))
        {
            nearestDeg = n * step;
        }
        else
        {
            nearestDeg = (n + 1) * step;
        }
        //Console.WriteLine("Nearest: "+nearestDeg);
        return nearestDeg;
    }

    private static bool isInViewport(AzimuthalCoordinate pos, Fov fov, AzimuthalCoordinate viewportPos)
    {
        if ( viewportPos.Azimuth - (fov.Width/2) < pos.Azimuth 
            &&
            viewportPos.Azimuth + (fov.Width/2) > pos.Azimuth 
            &&
            viewportPos.Height - (fov.Height/2) < pos.Height
            &&
            viewportPos.Height + (fov.Height/2) > pos.Height
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    
    
    public static List<Viewport> CalculateViewports(this List<Galaxy> galaxies, Fov fov, double rasterApprox)
    {
        Dictionary<Tuple<double, double>, Viewport> viewports = new Dictionary<Tuple<double, double>, Viewport>();

        
        
        double xStep = fov.Width * rasterApprox;
        double yStep = fov.Height * rasterApprox;

        int nXStep = (int)(fov.Width / xStep)/2;
        int nYStep = (int)(fov.Height / yStep)/2;
        
        Console.WriteLine(fov.Height);
        Console.WriteLine(fov.Width);

        Console.WriteLine(xStep);
        Console.WriteLine(yStep);
        
        IEnumerable<Galaxy> galaxiesAzimut = UgcDataRepo.New()
            .Galaxies.Select(g =>
                {
                    g.AzimuthalCoordinate =
                        g.EquatorialCoordinate.ToAzimuthal(new DateTime(), new Location(){Latitude = 47, Longitude = 12});
                    return g;
                }
            );
        
        int n = 1;
        foreach(Galaxy galaxy in galaxiesAzimut)
        {
            n += 1;
            
            Console.WriteLine(n);
            double xApprox = getNearestDeg(galaxy.AzimuthalCoordinate.Value.Azimuth, xStep);
            double yApprox = getNearestDeg(galaxy.AzimuthalCoordinate.Value.Height, yStep);
            try
            {
                Viewport viewport = viewports[new Tuple<double, double>(xApprox, yApprox)];
                viewport.Galaxies.Add(galaxy);
            }
            catch (Exception e)
            {
                Viewport viewport = new Viewport();
                viewport.pos = (new AzimuthalCoordinate() { Azimuth = xApprox, Height = yApprox });
                viewport.Galaxies.Add(galaxy);
                viewports[new Tuple<double, double>(xApprox, yApprox)] = viewport;
            }

            for (double ySearch = (yApprox - nYStep * yStep); ySearch < (yApprox + nYStep * yStep); ySearch+=yStep)
            {
                for (double xSearch = (xApprox - nXStep * xStep); xSearch < (xApprox + nXStep * xStep); xSearch+=xStep)
                {
                   
                    if (!isInViewport(galaxy.AzimuthalCoordinate.Value,fov,new AzimuthalCoordinate() { Azimuth = xSearch, Height = ySearch })
                        ||
                        ((Math.Abs(xSearch - xApprox) > 0.000002 && Math.Abs(ySearch - yApprox) > 0.000002)))
                    {
                        continue;
                    }
                    try
                    {
                        Viewport viewport = viewports[new Tuple<double, double>(xSearch, ySearch)];
                        viewport?.Galaxies.Add(galaxy);
                    }catch (Exception e)
                    {
                        Viewport viewport = new Viewport();
                        viewport.pos = (new AzimuthalCoordinate() { Azimuth = xApprox, Height = yApprox });
                        viewport.Galaxies.Add(galaxy);
                        
                        viewports[new Tuple<double, double>(xSearch, ySearch)] = viewport;
                    }
                }
            }
            
        }
        
        
        foreach(Galaxy galaxy in galaxiesAzimut)
        {
            Console.WriteLine(n);
            double xApprox = getNearestDeg(galaxy.AzimuthalCoordinate.Value.Azimuth, xStep);
            double yApprox = getNearestDeg(galaxy.AzimuthalCoordinate.Value.Height, yStep);

            for (double ySearch = (yApprox - nYStep * yStep); ySearch < (yApprox + nYStep * yStep); ySearch+=yStep)
            {
                for (double xSearch = (xApprox - nXStep * xStep); xSearch < (xApprox + nXStep * xStep); xSearch+=xStep)
                {
                    if (!isInViewport(galaxy.AzimuthalCoordinate.Value,fov,new AzimuthalCoordinate() { Azimuth = xSearch, Height = ySearch })
                        ||
                        ((Math.Abs(xSearch - xApprox) > 0.000002 && Math.Abs(ySearch - yApprox) > 0.000002)))
                    {
                        continue;
                    }
                    try
                    {
                        if ( viewports[new Tuple<double, double>(xSearch, ySearch)].Galaxies.Count<1)
                        {
                            viewports.Remove(new Tuple<double, double>(xSearch, ySearch));
                        }
                    }catch (Exception e)
                    {
                    }
                }
            }
            
        }
        
        return new List<Viewport>(viewports.Values);
    }
}