using Newtonsoft.Json;
using System;

namespace TSP
{
    // Represents a city, which is a node in the Traveling Salesman Problem
    class City
    {
        // Constants
        private const double SCALE_FACTOR = 1000;
        // This makes distances asymmetric
        // 0 <= Max elevation <= 1
        public const double MAX_ELEVATION = 0.10;  

        public double x;
        public double y;
        private double _elevation;
        // The mode manager applies to all the cities
        private static HardMode modeManager;

        public City(double x, double y)
        {
            this.x = x;
            this.y = y;
            _elevation = 0.0;
        }

        [JsonConstructorAttribute]
        public City(double x, double y, double elevation)
        {
            this.x = x;
            this.y = y;
            _elevation = elevation;
        }

        // How much does it cost to get from this city to the destination?
        // Note that this is an asymmetric cost function.
        // In advanced mode, it returns infinity when there is no connection.
        public double costToGetTo (City destination) 
        {
            // Cartesian distance
            double magnitude = Math.Sqrt(Math.Pow(this.x - destination.x, 2) + Math.Pow(this.y - destination.y, 2));

            // For Medium and Hard modes, add in an asymmetric cost (in easy mode it is zero).
            magnitude += (destination._elevation - this._elevation);
            if (magnitude < 0.0) magnitude = 0.0;

            magnitude *= SCALE_FACTOR;

            // In hard mode, remove edges; this slows down the calculation...
            if (modeManager.isEdgeRemoved(this,destination))
                magnitude = Double.PositiveInfinity;

            return Math.Round(magnitude);
        }

        public static void setModeManager(HardMode modeManager)
        {
            City.modeManager = modeManager;
        }

    }
}
