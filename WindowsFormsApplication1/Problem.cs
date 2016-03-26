using System;
using System.Collections.Generic;

namespace TSP
{
    class Problem
    {
        // Default values
        public const int DEFAULT_SEED = 1;
        public const int DEFAULT_PROBLEM_SIZE = 20;
        public const int DEFAULT_TIME_LIMIT = 60;        // in seconds
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        // Class members
        public int seed;
        public int size;
        public int timeLimit;
        public HardMode.Modes mode;
        private Random rnd;

        public City[] cities;
        public TSPSolution bssf;
        public TimeSpan timeElasped;
        public int solutions;

        // Constructors
        public Problem()
        {
            initialize(DEFAULT_SEED, DEFAULT_PROBLEM_SIZE, DEFAULT_TIME_LIMIT, HardMode.Modes.Hard);
        }

        public Problem(int seed, int size, int timeInSeconds, HardMode.Modes mode)
        {
            initialize(seed, size, timeInSeconds, mode);
        }

        private void initialize(int seed, int problemSize, int timeInSeconds, HardMode.Modes mode)
        {
            this.seed = seed;
            this.rnd = new Random(seed);

            // CRITICAL - TO MAKE THE POINTS LOOK LIKE THE
            // POINTS IN THE OLD VERSION FOR THE SAME SEEDS
            // DO NOT REMOVE THIS FOR LOOP
            for (int i = 0; i < 50; i++)
                rnd.NextDouble();

            this.size = problemSize;
            this.mode = mode;
            this.timeLimit = timeInSeconds * 1000;                        // timer wants timeLimit in milliseconds
            this.resetData();
        }

        // Reset the problem instance
        private void resetData()
        {
            cities = new City[size];
            bssf = null;

            if (mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < size; i++)
                    cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < size; i++)
                    cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this.mode, this.rnd, cities);
            if (mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);
        }

        //  Return the cost of the best solution so far. 
        public double costOfBssf()
        {
            if (bssf != null)
            {
                // go through each edge in the route and add up the cost. 
                int x;
                List<City> route = bssf.route;
                City here;
                double cost = 0D;

                for (x = 0; x < route.Count - 1; x++)
                {
                    here = route[x];
                    cost += here.costToGetTo(route[x + 1]);
                }

                // go from the last city to the first. 
                here = route[route.Count - 1];
                cost += here.costToGetTo(route[0]);
                return cost;
            }
            else
                return -1D; 
        }
    }
}
