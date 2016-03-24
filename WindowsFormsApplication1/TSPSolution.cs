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
        private List<City> route;

        // <param name="iroute">a (hopefully) valid tour</param>
        public TSPSolution(List<City> iroute)
        {
            route = new List<City>(iroute);
        }

        public List<City> Route
        {
            get { return route; }
            set { route = value; }
        }

        // Compute the cost of the current route.  
        // Note: This does not check that the route is complete.
        // It assumes that the route passes from the last city back to the first city. 
        public double costOfRoute()
        {
            // go through each edge in the route and add up the cost. 
            int x;
            City here;
            double cost = 0D;

            for (x = 0; x < route.Count - 1; x++)
            {
                here = route[x] as City;
                cost += here.costToGetTo(route[x + 1] as City);
            }

            // go from the last city to the first. 
            here = route[route.Count - 1];
            cost += here.costToGetTo(route[0]);
            return cost;
        }
    }
}
