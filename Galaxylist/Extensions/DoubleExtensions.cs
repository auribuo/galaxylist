namespace Galaxylist.Lib.Extensions;


public static partial class Extensions
{

    public static Declination ToDeclination(this double declinationDegree)
    {
        double degrees = Math.Floor(declinationDegree);
        double minutes = Math.Floor((declinationDegree-degrees)*60);
        double seconds= (declinationDegree - degrees - minutes/60) * 3600;
        Declination declination = new Declination{Degrees = degrees,Minutes = minutes,Seconds = seconds};
        
        return declination;
    }

    public static RightAscention ToRightAscention(this double rightAscentionDegree)
    {
        double inHours = rightAscentionDegree * 24 / 360;
        double hours = Math.Floor(inHours);
        double minutes =  Math.Floor((inHours-hours)*60);
        double seconds= (inHours - hours - minutes/60)*3600;

        RightAscention rightAscention = new RightAscention { Hours = hours, Minutes = minutes,Seconds = seconds};

        return rightAscention;
    }
}