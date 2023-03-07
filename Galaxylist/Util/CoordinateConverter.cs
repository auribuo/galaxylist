namespace Galaxylist.Util;


// https://de.wikipedia.org/wiki/Sternzeit#Sternzeit_in_Greenwich
//TODO Andere Orte Testen
public static   class CoordinateConverter
{

    /// <summary>
    /// Konvertiert äquatoriale Koordinaten in vom Standpunkt abhängige Horizontkoordinaten
    /// </summary>
    /// <param name="dateTime">Zeitpunkt für Berechnung. WICHTIG: UTC!!</param>
    /// <param name="longitude">Längengrad des Standortes</param>
    /// <param name="altitude">Breitengrad des Standortes</param>
    /// <param name="rektaszension">Rektazension der äquatorialen Koordinaten</param>
    /// <param name="deklination">Deklination der äquatorialen Koordinaten</param>
    /// <returns>Gibt Tupel der Horizontkoordinaten (Hoehenwinkel, Azimut) in Grad zurück</returns>
    public static  Tuple<double, double> ConvEqToZen(DateTime dateTime,  double longitude, double altitude,  double rektaszension, double deklination)
    {
        var julianDate = ToJulianDate(dateTime);
        var t = JulianDateDifferenceConstant(julianDate);
        var gmst0 = GMST(t);
        
        
        // Konvertiere Uhrzeit in Grad
        var timeOfDayDeg = (double)dateTime.Hour / 24 * 360 ;
        
        // Skaliere Uhrzeit auf Sternzeit
        var timeOfDayRect = timeOfDayDeg * 1.00273790935;
        
        // Addiere Uhrzeit zu Greenwich Sternzeit zum Zeitpunkt 0
        var gmstT = (gmst0 + timeOfDayRect) % 360;

        
        var lmst = LMST(gmstT, longitude);
        var stundenWinkel = (lmst - rektaszension);
        

        
        // Berechnet Höhenwinkel in Radianten
        var hoehenWinkel = Math.Asin(
            Math.Sin(DegToRad(deklination))  * Math.Sin(DegToRad(altitude))
            +
            Math.Cos(DegToRad(altitude)) * Math.Cos(DegToRad(deklination)) * Math.Cos(DegToRad(stundenWinkel))
        );

        // Berechnet Azimut in Radianten
        var azimut = Math.Atan(
            Math.Sin(DegToRad(stundenWinkel))/
            (
                Math.Sin(DegToRad(altitude)) * Math.Cos(DegToRad(stundenWinkel))
                -
                Math.Cos(DegToRad(altitude)) * Math.Tan(DegToRad(deklination))
             )
        );

        
        // Kontrolliert ob sich Stundenwinkel und Azimut im selben Quadranten befinden.
        // Ist dies der Fall, werden dem Azimut 180° addiert. (Arctan hat 2 Lösungen)
        var azimutDeg = azimut * 180 / Math.PI;
        if (GetQudrant(azimutDeg) == GetQudrant(stundenWinkel))
        {
            azimutDeg = (azimutDeg + 180) % 360;
        }
        
        /*Console.WriteLine("T: "+ t);
        Console.WriteLine("Greenwich time: "+ gmstT);
        Console.WriteLine("Datum: "+ julianDate);
        Console.WriteLine("Deklination: "+ deklination);
        Console.WriteLine("Rektazension: "+ rektazension);
        Console.WriteLine("Sternzeit: "+lmst);
        Console.WriteLine("Stundenwinkel: "+stundenWinkel);
        Console.WriteLine("Hoehenwinkel in Rad: "+hoehenWinkel);
        Console.WriteLine("Azimut in Rad: "+azimut);
        Console.WriteLine("Hohenwinkel in Grad: "+ RadToDeg(hoehenWinkel));
        Console.WriteLine("Azimut in Grad: "+ azimutDeg);*/

        return new Tuple<double, double>(RadToDeg(hoehenWinkel), azimutDeg);
    }


    /// <summary>
    /// Berechnet Quadrant eines Winkels
    /// </summary>
    /// <returns>Quadrant des Winkels</returns>
    private static int GetQudrant(double degree)
    {
        switch (degree%360)
        {
            case >= 0 and < 90:
                return 1;
            case >= 90  and < 180 :
                return 2;
            case >= 180  and <270:
                return 3;
            case >=270 and <360:
                return 4;
        }
        return 1;
    }

    /// <summary>
    /// Konvertiert Grad in Radianten
    /// </summary>
    private static double DegToRad(double degree)
    {
        return degree * Math.PI / 180;
    }
    
    /// <summary>
    /// Konvertiert Grad in Radianten
    /// </summary>
    private static double RadToDeg(double radians)
    {
        return radians * 180 / Math.PI;
    }
    
    /// <summary>
    /// Errechnet Sternzeit am Standort des Beobachters
    /// </summary>
    /// <param name="gmst">Sternzeit in Greenwich [°]</param>
    /// <param name="longitude">Längengrad des Beobachters [°]</param>
    /// <returns>Sternzeit beim Beobachter [°]</returns>
    private static double LMST(double gmst, double longitude)
    {
        return (gmst  + longitude) ;
    }

    /// <summary>
    /// Konvertiert Grad, Gradminuten und Gradsekunden in Grad um
    /// </summary>
    public static double DegreeMinSecToDegree(double degree, double degreeMinutes, double degreeSeconds)
    {
        if (degree < 0)
        {
            return degree - (degreeMinutes / 60 + degreeSeconds / 3600);   
        }
        return degree + degreeMinutes / 60 + degreeSeconds / 3600;
    }
    
    /// <summary>
    /// Berechnet Greenwicher Sternzeit aus T 
    /// </summary>
    /// <param name="T">Parameter T (siehe Wikipedia)</param>
    /// <returns>Gibt Greenwicher Sternzeit zurück [°]</returns>
    private static double GMST(double T)
    {
        return 100.46061837 + 36000.770053608 * T + 0.000387933 * T * T - (T*T*T /38710000);
    }
    
    
    /// <summary>
    /// Konvertiert ein Datum in die julianische Zeitrechnung
    /// </summary>
    /// <returns>Datum in julianischer Zeitrechnung</returns>
    private static double ToJulianDate(DateTime date)
    {
        // ToOADate konvertiert ein Datum zu OA Datum mit Referenz zum 30. Dezember 1899
        // Das julianische Datum bezieht sich aber auf 4713 vor Christus.
        // Diese Verschiebung wird mitmit +241018.5 berücksichtigt.
        return date.ToOADate() + 2415018.5;
    }
    
    /// <summary>
    /// Berechnet T aus [Sternzeit in Greenwich] (https://de.wikipedia.org/wiki/Sternzeit#Sternzeit_in_Greenwich)
    /// </summary>
    /// <param name="julianDate">Datum in julianischer Zeitrechnung</param>
    /// <returns>Variable T</returns>
    private static double JulianDateDifferenceConstant(double julianDate)
    {
        return (julianDate - 2451545.0) / 36525;
    }
    
    
    /// <summary>
    /// Konvertiert Stunden, Minuten und Sekunden in Grad
    /// </summary>
    public static double HourMinSecToDegree(double hours, double minutes, double seconds)
    {
        return (hours * 3600 + minutes * 60 + seconds) * 360 / 86400;
    }
}