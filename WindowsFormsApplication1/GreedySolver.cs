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
        private const double SPACE_PLACEHOLDER = double.PositiveInfinity;

        private static Problem cityData;
        private static State state;

        // finds the greedy tour starting from each city and keeps the best (valid) one
        // <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution,
        // number of solutions found during search (not counting initial BSSF estimate)</returns>
        // For an example of what to return, see DefaultSolver.solve() method.
        public static string[] solve(Problem cityData2)
        {
            Stopwatch timer = new Stopwatch();
            cityData = cityData2;
            state = new State(cityData.Size);

            timer.Start();
            initializeState();
            recur(state.startCityIndex);
            timer.Stop();

            // From the list of city indexes, create the list for the BSSF
            List<City> citiesVisitedInOrder = new List<City>(state.cityOrder.Count);
            foreach(int i in state.cityOrder)
                citiesVisitedInOrder.Add(cityData.Cities[i]);
            // Since cityData is static, it is pointing to the cityData object
            // that was passed in. Setting it's BSSF here will set the BSSF 
            // of the cityData object in the Form1 class.
            cityData.BSSF = new TSPSolution(citiesVisitedInOrder);

            string[] rtn = new string[3];
            rtn[Problem.COST_POSITION] = state.lowerBound.ToString();
            rtn[Problem.TIME_POSITION] = timer.Elapsed.ToString();
            rtn[Problem.COUNT_POSITION] = "1";

            // Null out some variables before exiting
            state = null;
            cityData = null;
            return rtn;
        }

        private static void initializeState()
        {

            // Populate the state matrix with distances between cities
            for(int i = 0; i < cityData.Size; i++)
            {
                City city = cityData.Cities[i];
                for(int j = 0; j < cityData.Size; j++)
                {
                    if (i == j)
                        state.matrix[i, j] = SPACE_PLACEHOLDER;
                    else
                        state.matrix[i, j] = city.costToGetTo(cityData.Cities[j]);
                }
            }

            reduce();
        }

        private static void reduce()
        {
            // Reduce the rows
            for (int i = 0; i < state.matrix.GetLength(0); i++)
            {
                // Find the smallest value in the row
                double minInRow = double.PositiveInfinity;
                for (int j = 0; j < state.matrix.GetLength(1); j++)
                    minInRow = Math.Min(minInRow, state.matrix[i,j]);

                // If the smallest value in the row is 0,
                // then there already is a 0 in the row
                // We don't need to add a 0 into the row
                //
                // If the smallest value in the row is infinity,
                // then we have a row of infinities
                // Nothing happens
                if (minInRow != 0 && minInRow != SPACE_PLACEHOLDER)
                {
                    // Add the smallest value in the row to the lower bound
                    state.lowerBound += minInRow;

                    // Subtract that value from all the values in the row
                    // The [i,j] that contained the smallest value will be
                    // set to 0, bc minInRow - minInRow = 0;
                    // If the value at [i,j] is the placeholder, nothing happens
                    for (int j = 0; j < state.matrix.GetLength(1); j++)
                        if(state.matrix[i,j] != SPACE_PLACEHOLDER)
                            state.matrix[i, j] -= minInRow;
                }

            }

            // Reduce the columns
            for(int j = 0; j < state.matrix.GetLength(1); j++)
            {
                // Find the smallest value in the column
                double minInColumn = double.PositiveInfinity;
                for (int i = 0; i < state.matrix.GetLength(0); i++)
                    minInColumn = Math.Min(minInColumn, state.matrix[i, j]);

                // Same as above, except switching rows for columns
                if(minInColumn != 0 && minInColumn != SPACE_PLACEHOLDER)
                {
                    // Add the smallest value in the column to the lower bound
                    state.lowerBound += minInColumn;

                    // Subtract that value from all the values in the column
                    for (int i = 0; i < state.matrix.GetLength(0); i++)
                        if(state.matrix[i,j] != SPACE_PLACEHOLDER)
                            state.matrix[i, j] -= minInColumn;
                }
            }
        }

        // Returns the index of the last city to be visited in the cycle
        private static void recur(int currCityIndex)
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
                state.matrix[currCityIndex, j] = SPACE_PLACEHOLDER;

            // column j
            for (int i = 0; i < state.matrix.GetLength(0); i++)
                state.matrix[i, closestCityIndex] = SPACE_PLACEHOLDER;

            // [j,i]
            state.matrix[closestCityIndex, currCityIndex] = SPACE_PLACEHOLDER;

            // reduce
            reduce();

            recur(closestCityIndex);
        }
    }
}
