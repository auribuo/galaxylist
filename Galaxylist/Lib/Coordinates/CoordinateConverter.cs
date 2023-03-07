namespace Galaxylist.Lib.Coordinates;

using Models;

// https://de.wikipedia.org/wiki/Sternzeit#Sternzeit_in_Greenwich
//TODO Andere Orte Testen
public static class CoordinateConverter
{
	/// <summary>
	/// Konvertiert äquatoriale Koordinaten in vom Standpunkt abhängige Horizontkoordinaten
	/// </summary>
	/// <param name="dateTime">Zeitpunkt für Berechnung. WICHTIG: UTC!!</param>
	/// <param name="longitude">Längengrad des Standortes</param>
	/// <param name="altitude">Breitengrad des Standortes</param>
	/// <param name="rightAscention">Rektazension der äquatorialen Koordinaten</param>
	/// <param name="declination">Deklination der äquatorialen Koordinaten</param>
	/// <returns>Gibt Tupel der Horizontkoordinaten (Hoehenwinkel, Azimut) in Grad zurück</returns>
	public static AzimuthalCoordinate ConvEqToZen(
		DateTime dateTime,
		double longitude,
		double altitude,
		RightAscention rightAscention,
		Declination declination
	)
	{
		double rektaszension = rightAscention.ToDegree();
		double deklination = declination.ToDegrees();
		double julianDate = ToJulianDate(dateTime);
		double t = JulianDateDifferenceConstant(julianDate);
		double gmst0 = Gmst(t);

		// Konvertiere Uhrzeit in Grad
		double timeOfDayDeg = (double)dateTime.Hour / 24 * 360;

		// Skaliere Uhrzeit auf Sternzeit
		double timeOfDayRect = timeOfDayDeg * 1.00273790935;

		// Addiere Uhrzeit zu Greenwich Sternzeit zum Zeitpunkt 0
		double gmstT = (gmst0 + timeOfDayRect) % 360;
		double lmst = Lmst(gmstT, longitude);
		double stundenWinkel = (lmst - rektaszension);

		// Berechnet Höhenwinkel in Radianten
		double hoehenWinkel = Math.Asin(Math.Sin(DegToRad(deklination)) * Math.Sin(DegToRad(altitude)) +
										Math.Cos(DegToRad(altitude)) * Math.Cos(DegToRad(deklination)) * Math.Cos(DegToRad(stundenWinkel))
		);

		// Berechnet Azimut in Radianten
		double azimut = Math.Atan(Math.Sin(DegToRad(stundenWinkel)) / (Math.Sin(DegToRad(altitude)) * Math.Cos(DegToRad(stundenWinkel)) -
																	   Math.Cos(DegToRad(altitude)) * Math.Tan(DegToRad(deklination)))
		);

		// Kontrolliert ob sich Stundenwinkel und Azimut im selben Quadranten befinden.
		// Ist dies der Fall, werden dem Azimut 180° addiert. (Arctan hat 2 Lösungen)
		double azimutDeg = azimut * 180 / Math.PI;

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

		return new AzimuthalCoordinate
		{
			Height = RadToDeg(hoehenWinkel),
			Azimuth = azimutDeg,
		};
	}

	/// <summary>
	/// Berechnet Quadrant eines Winkels
	/// </summary>
	/// <returns>Quadrant des Winkels</returns>
	private static int GetQudrant(double degree)
	{
		return (degree % 360) switch
		{
			>= 0 and < 90    => 1,
			>= 90 and < 180  => 2,
			>= 180 and < 270 => 3,
			>= 270 and < 360 => 4,
			var _            => 1,
		};
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
	private static double Lmst(double gmst, double longitude)
	{
		return gmst + longitude;
	}

	/// <summary>
	/// Berechnet Greenwicher Sternzeit aus T 
	/// </summary>
	/// <param name="T">Parameter T (siehe Wikipedia)</param>
	/// <returns>Gibt Greenwicher Sternzeit zurück [°]</returns>
	private static double Gmst(double T)
	{
		return 100.46061837 + 36000.770053608 * T + 0.000387933 * T * T - (T * T * T / 38710000);
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
}