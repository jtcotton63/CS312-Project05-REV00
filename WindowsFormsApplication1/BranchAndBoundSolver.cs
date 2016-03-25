﻿using System;
using System.Collections.Generic;

namespace TSP
{
    /////////////////////////////////////////////////////////////////////////////////////////////
    // Project 5
    ////////////////////////////////////////////////////////////////////////////////////////////

    /*
     * Implement your branch and bound solver in the solve() method of this class.
     */
    class BranchAndBoundSolver
    {
        Problem cityData;
        Queue<State> q;

        public BranchAndBoundSolver(Problem cityData)
        {
            this.cityData = cityData;
            q = new Queue<State>();
        }
        
        // finds the best tour possible using the branch and bound method of attack
        // For an example of what to return, see DefaultSolver.solve() method.
        public Problem solve()
        {
            State beginningState = new State(cityData.size);
            SolverHelper.initializeState(beginningState, cityData);
            q.Enqueue(beginningState);
            Problem determineBSSFProblem = SolverHelper.clone(cityData);
            this.cityData.bssf = GreedySolver.solve(determineBSSFProblem).bssf;

            while (q.Count > 0)
            {
                recur(q.Dequeue());
                //Clear out any states that have a BSSF less than 
                //that of the current
            }

            return cityData;
        }

        private void recur(State state)
        {
            state.addCity(state.currCityIndex);

            // This is a leaf
            // All cities have been visited; finish and return
            if (state.getCitiesVisited() == state.size - 1)
            {
                if (state.matrix[state.currCityIndex, state.cityOfOriginIndex] == double.PositiveInfinity)
                    throw new SystemException("This should never happen");

                // Since we've hit a leaf node, we've found another possible solution
                cityData.solutions++;

                // Add the last hop to the lower bound to get the cost of the complete cycle
                state.lowerBound += state.matrix[state.currCityIndex, state.cityOfOriginIndex];

                // Check to see if this route is smaller than the current BSSF
                // If so, replace the current BSSF
                if (state.lowerBound < cityData.bssf.costOfRoute)
                    cityData.bssf = new TSPSolution(cityData.cities, state.cityOrder, state.lowerBound);

                return;
            }

            for (int j = 1; j < state.matrix.GetLength(1); j++)
            {
                Console.WriteLine(state.currCityIndex + " " + j);
                state.printState();

                double currDistVal = state.matrix[state.currCityIndex, j];
                if (currDistVal != SolverHelper.SPACE_PLACEHOLDER &&
                    state.lowerBound + currDistVal < cityData.bssf.costOfRoute)
                {
                    State childState = SolverHelper.clone(state);

                    // add [i,j]
                    childState.lowerBound += currDistVal;

                    // Blank out the appropriate values with the placeholder
                    SolverHelper.blankOut(childState, j);
                    childState.printState();

                    // reduce
                    SolverHelper.reduce(childState);

                    childState.currCityIndex = j;

                    if(childState.lowerBound < cityData.bssf.costOfRoute)
                        q.Enqueue(childState);
                }
            }
        }
    }
}
