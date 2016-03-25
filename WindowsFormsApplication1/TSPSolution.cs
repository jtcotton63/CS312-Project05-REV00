using System;
using System.Collections.Generic;

namespace TSP
{
    class TSPSolution
    {
        // we use the representation [cityB,cityA,cityC] 
        // to mean that cityB is the first city in the solution, cityA is the second, cityC is the third 
        // and the edge from cityC to cityB is the final edge in the path.  
        // You are, of course, free to use a different representation if it would be more convenient or efficient 
        // for your data structure(s) and search algorithm. 
        public List<City> route;
        public TimeSpan timeElasped;
        public double costOfRoute;

        public TSPSolution(City[] cities, List<int> cityOrder, double costOfRoute)
        {
            // From the list of city indexes, create the list of cities
            route = new List<City>(cityOrder.Count);
            foreach (int i in cityOrder)
            {
                City temp = SolverHelper.clone(cities[i]);
                route.Add(temp);
            }
            this.costOfRoute = costOfRoute;
        }
        
        // <param name="iroute">a (hopefully) valid tour</param>
        public TSPSolution(List<City> iroute)
        {
            route = new List<City>(iroute);
        }
    }
}
