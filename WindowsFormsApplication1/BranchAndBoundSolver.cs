using System;

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
        Problem cityData;

        public BranchAndBoundSolver(Problem cityData)
        {
            this.cityData = cityData;
        }

        // finds the best tour possible using the branch and bound method of attack
        // <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution,
        // number of solutions found during search (not counting initial BSSF estimate)</returns>
        // For an example of what to return, see DefaultSolver.solve() method.
        public string[] solve()
        {
            throw new NotImplementedException();
        }
    }
}
