using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TSP
{
    class DefaultSolver : Solver
    {
        // This is the entry point for the default solver
        // which just finds a valid random tour 
        // <returns>The random valid tour</returns>
        public Problem solve(Problem cityData)
        {
            int i, swap, temp = 0;
            City[] cities = cityData.Cities;
            string[] results = new string[3];
            int[] perm = new int[cities.Length];
            List<City> route = new List<City>();
            Random rnd = new Random();
            Stopwatch timer = new Stopwatch();

            timer.Start();
            do
            {
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = rnd.Next(0, cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                route.Clear();
                for (i = 0; i < cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    route.Add(cities[perm[i]]);
                }
                cityData.BSSF = new TSPSolution(route);
            } while (cityData.costOfBssf() == double.PositiveInfinity);                // until a valid route is found
            timer.Stop();

            cityData.BSSF.costOfRoute = cityData.costOfBssf();
            cityData.BSSF.timeElasped = timer.Elapsed;
            cityData.Solutions = 1;

            return cityData;
        }
    }
}
