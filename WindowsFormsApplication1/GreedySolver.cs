using System;
using System.Collections.Generic;
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
            State state = new State(cityData.Size);

            timer.Start();
            SolverHelper.initializeState(state, cityData);
            recur(state, state.startCityIndex);
            timer.Stop();

            // Since cityData is static, it is pointing to the cityData object
            // that was passed in. Setting it's BSSF here will set the BSSF 
            // of the cityData object in the Form1 class.
            cityData.BSSF = new TSPSolution(cityData.Cities, state.cityOrder);
            cityData.BSSF.costOfRoute = state.lowerBound;
            cityData.BSSF.timeElasped = timer.Elapsed;
            cityData.Solutions = 1;

            return cityData;
        }

        // Returns the index of the last city to be visited in the cycle
        public static void recur(State state, int currCityIndex)
        {            
            // All cities have been visited; finish and return
            if(state.getCitiesVisited() == state.size - 1)
            {
                if (state.matrix[currCityIndex, state.startCityIndex] == double.PositiveInfinity)
                    throw new SystemException("This should never happen");

                state.addCity(currCityIndex);
                state.lowerBound += state.matrix[currCityIndex, state.startCityIndex];
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
            // row i
            for (int j = 0; j < state.matrix.GetLength(1); j++)
                state.matrix[currCityIndex, j] = SolverHelper.SPACE_PLACEHOLDER;

            // column j
            for (int i = 0; i < state.matrix.GetLength(0); i++)
                state.matrix[i, closestCityIndex] = SolverHelper.SPACE_PLACEHOLDER;

            // [j,i]
            state.matrix[closestCityIndex, currCityIndex] = SolverHelper.SPACE_PLACEHOLDER;

            // reduce
            SolverHelper.reduce(state);

            recur(state, closestCityIndex);
        }
    }
}
