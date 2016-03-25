using System;
using System.Diagnostics;

namespace TSP
{
    /////////////////////////////////////////////////////////////////////////////////////////////
    // Project 6
    ////////////////////////////////////////////////////////////////////////////////////////////

    /*
     * Implement the greedy solver in the solve() method of this class.
     */
    static class GreedySolver
    {
        // finds the greedy tour starting from each city and keeps the best (valid) one
        // For an example of what to return, see DefaultSolver.solve() method.
        public static Problem solve(Problem cityData)
        {
            Stopwatch timer = new Stopwatch();
            State state = new State(cityData.size);

            timer.Start();
            SolverHelper.initializeState(state, cityData);
            state.currCityIndex = 0;
            recur(state);
            timer.Stop();

            // Since cityData is static, it is pointing to the cityData object
            // that was passed in. Setting it's BSSF here will set the BSSF 
            // of the cityData object in the Form1 class.
            cityData.bssf = new TSPSolution(cityData.cities, state.cityOrder, state.lowerBound);
            cityData.bssf.timeElasped = timer.Elapsed;
            cityData.solutions = 1;

            return cityData;
        }

        // Returns the index of the last city to be visited in the cycle
        private static void recur(State state)
        {
            int currCityIndex = state.currCityIndex;
            // All cities have been visited; finish and return
            if(state.getCitiesVisited() == state.size - 1)
            {
                if (state.matrix[currCityIndex, state.cityOfOriginIndex] == double.PositiveInfinity)
                    throw new SystemException("This should never happen");

                state.addCity(currCityIndex);
                state.lowerBound += state.matrix[currCityIndex, state.cityOfOriginIndex];
                return;
            }

            state.addCity(currCityIndex);
            
            // find the smallest distance to any other city
            double closestCityDistance = double.PositiveInfinity;
            int closestCityIndex = -1;
            for (int j = 1; j < state.matrix.GetLength(1); j++)
                // For a different starting city, the following code
                // would have to be added:
                //for (int j = 0; j < state.matrix.GetLength(1); j++)
                //if (j == state.startCityIndex)
                //    continue;
                if (state.matrix[currCityIndex, j] < closestCityDistance)
                {
                    closestCityIndex = j;
                    closestCityDistance = state.matrix[currCityIndex, j];
                }

            // add [i,j]
            state.lowerBound += closestCityDistance;

            // Blank out appropriate spaces
            SolverHelper.blankOut(state, closestCityIndex);

            // reduce
            SolverHelper.reduce(state);

            state.currCityIndex = closestCityIndex;
            recur(state);
        }
    }
}
