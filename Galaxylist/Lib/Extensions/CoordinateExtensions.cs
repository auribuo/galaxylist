namespace Galaxylist.Lib.Extensions;

// https://de.wikipedia.org/wiki/Sternzeit#Sternzeit_in_Greenwich
/// <summary>
/// Class holding all extension methods
/// </summary>
public static partial class Extensions
{
	/// <summary>
	/// Converts a <see cref="EquatorialCoordinate"/> to a <see cref="AzimuthalCoordinate"/> dependent of the location and time.
	/// </summary>
	/// <param name="coordinate">The equatorial coordinate to convert</param>
	/// <param name="dateTime">Time of the calculation. Will be converted to UTC automatically</param>
	/// <param name="location">Location of the calculation</param>
	/// <returns>The equivalent azimuthal coordinate based on the given location and time</returns>
	public static AzimuthalCoordinate ToAzimuthal(this EquatorialCoordinate coordinate, DateTime dateTime, Location location)
	{
		dateTime = dateTime.ToUniversalTime();
		double longitude = location.Longitude;
		double latitude = location.Latitude;
		double rektaszension = coordinate.RightAscention.ToDegree();
		double deklination = coordinate.Declination.ToDegrees();
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
		double stundenWinkel = lmst - rektaszension;

		// Berechnet Höhenwinkel in Radianten
		double hoehenWinkel = Math.Asin(Math.Sin(DegToRad(deklination)) * Math.Sin(DegToRad(latitude)) +
										Math.Cos(DegToRad(latitude)) * Math.Cos(DegToRad(deklination)) * Math.Cos(DegToRad(stundenWinkel))
		);

		// Berechnet Azimut in Radianten
		double azimut = Math.Atan(Math.Sin(DegToRad(stundenWinkel)) / (Math.Sin(DegToRad(latitude)) * Math.Cos(DegToRad(stundenWinkel)) -
																	   Math.Cos(DegToRad(latitude)) * Math.Tan(DegToRad(deklination)))
		);

		// Kontrolliert ob sich Stundenwinkel und Azimut im selben Quadranten befinden.
		// Ist dies der Fall, werden dem Azimut 180° addiert. (Arctan hat 2 Lösungen)
		double azimutDeg = RadToDeg(azimut);
		azimutDeg = (azimutDeg + 360) % 360;

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
	/// Calculates the quadrant of an angle
	/// </summary>
	/// <returns>The quadrant of the given angle</returns>
	private static int GetQudrant(double angle)
	{
		return (angle % 360) switch
		{
			>= 0 and < 90    => 1,
			>= 90 and < 180  => 2,
			>= 180 and < 270 => 3,
			>= 270 and < 360 => 4,
			var _            => 1,
		};
	}

	/// <summary>
	/// Converts the given angle in degrees to radians
	/// <returns>The given angle in radians</returns>
	/// </summary>
	private static double DegToRad(double angle)
	{
		return angle * Math.PI / 180;
	}

	/// <summary>
	/// Converts the given angle in radians to degrees
	/// <returns>The given angle in degrees</returns>
	/// </summary>
	private static double RadToDeg(double radians)
	{
		return radians * 180 / Math.PI;
	}

	/// <summary>
	/// Calculates the star time of the given location and time
	/// </summary>
	/// <param name="gmst">Star time in Greenwich in degrees</param>
	/// <param name="longitude">Longitude of the observer in degrees</param>
	/// <returns>Star time at the location of the observer in degrees</returns>
	private static double Lmst(double gmst, double longitude)
	{
		return gmst + longitude;
	}

	/// <summary>
	/// Calculats the Greenwich mean sidereal time from t
	/// </summary>
	/// <param name="t">Parameter T (see Wikipedia)</param>
	/// <returns>The Greenwich mean sidereal time in degrees</returns>
	private static double Gmst(double t)
	{
		return 100.46061837 + 36000.770053608 * t + 0.000387933 * t * t - (t * t * t / 38710000);
	}

	/// <summary>
	/// Converts a <see cref="DateTime"/> to a julian date
	/// </summary>
	/// <returns>Date in julian date</returns>
	private static double ToJulianDate(DateTime date)
	{
		// ToOADate konvertiert ein Datum zu OA Datum mit Referenz zum 30. Dezember 1899
		// Das julianische Datum bezieht sich aber auf 4713 vor Christus.
		// Diese Verschiebung wird mitmit +241018.5 berücksichtigt.
		return date.ToOADate() + 2415018.5;
	}

	/// <summary>
	/// Calculates the parameter T from a given julian date (https://de.wikipedia.org/wiki/Sternzeit#Sternzeit_in_Greenwich)
	/// </summary>
	/// <param name="julianDate">Julian date to use</param>
	/// <returns>Variable T</returns>
	private static double JulianDateDifferenceConstant(double julianDate)
	{
		return (julianDate - 2451545.0) / 36525;
	}
}