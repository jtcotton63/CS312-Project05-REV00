namespace TSP
{
    /////////////////////////////////////////////////////////////////////////////////////////////
    // Project 5
    ////////////////////////////////////////////////////////////////////////////////////////////

    /*
     * Implement your branch and bound solver in the solve() method of this class.
     */
    class BranchAndBoundSolver : Solver
    {
        // finds the best tour possible using the branch and bound method of attack
        // For an example of what to return, see DefaultSolver.solve() method.
        public Problem solve(Problem cityData)
        {
            State originalState = new State(cityData.Size);
            SolverHelper.initializeState(originalState, cityData);
            //q.Add(originalState);
            State determineBSSFState = SolverHelper.clone(originalState);
            cityData.BSSF = determineBSSF(determineBSSFState);


            //while (!q.empty())
            //  recur(q.pop());
            return null;
        }

        private TSPSolution determineBSSF(State state)
        {
            GreedySolver.recur(state, 0);
            return null;
        }
    }
}
