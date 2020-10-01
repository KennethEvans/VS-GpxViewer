using System;

namespace Exercise_Analyzer {
    class GpsUtils {
        /**
         * Nominal radius of the earth in miles. The radius actually varies from
         * 3937 to 3976 mi.
         */
        public const double REARTH = 3956;
        /** Multiplier to convert miles to nautical miles. */
        public const double MI2NMI = 1.852; // Exact
                                            /** Multiplier to convert degrees to radians. */
        public const double DEG2RAD = Math.PI / 180.0;
        /** Multiplier to convert feet to miles. */
        public const double FT2MI = 1.0 / 5280.0;
        /** Multiplier to convert meters to miles. */
        public const double M2MI = .00062137119224;
        /** Multiplier to convert kilometers to miles. */
        public const double KM2MI = .001 * M2MI;
        /** Multiplier to convert meters to feet. */
        public const double M2FT = 3.280839895;
        /** Multiplier to convert sec to hours. */
        public const double SEC2HR = 1.0 / 3600.0;
        /** Multiplier to convert millisec to hours. */
        public const double MS2HR = .001 * SEC2HR;

        /** String used for non-available data */
        private const String NOT_AVAILABLE = "NA";

        /**
         * The speed in m/sec below which there is considered to be no movement for
         * the purposes of calculating Moving Time. This is, of course, arbitrary.
         * Note that 1 mi/hr is 0.44704 m/sec. This is expected to be set from
         * preferences.
         */
        public static double NO_MOVE_SPEED = .5;

        /**
         * Returns great circle distance in meters. assuming a spherical earth. Uses
         * Haversine formula.
         * 
         * @param lat1 Start latitude in deg.
         * @param lon1 Start longitude in deg.
         * @param lat2 End latitude in deg.
         * @param lon2 End longitude in deg.
         * @return
         */
        public static double greatCircleDistance(double lat1, double lon1,
            double lat2, double lon2) {
            double slon, slat, a, c, d;

            // Convert to radians
            lat1 *= DEG2RAD;
            lon1 *= DEG2RAD;
            lat2 *= DEG2RAD;
            lon2 *= DEG2RAD;

            // Haversine formula
            slon = Math.Sin((lon2 - lon1) / 2.0);
            slat = Math.Sin((lat2 - lat1) / 2.0);
            a = slat * slat + Math.Cos(lat1) * Math.Cos(lat2) * slon * slon;
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            d = REARTH / M2MI * c;

            return (d);
        }
    }
}
