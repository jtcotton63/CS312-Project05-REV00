using System;

namespace TSP
{
    /////////////////////////////////////////////////////////////////////////////////////////////
    // Project 6
    ////////////////////////////////////////////////////////////////////////////////////////////
    
    /*
     * Implement the greedy solver in the solve() method of this class.
     */
    class GreedySolver : Solver
    {
        Problem cityData;

        public GreedySolver(Problem cityData)
        {
            this.cityData = cityData;
        }

        // finds the greedy tour starting from each city and keeps the best (valid) one
        // <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution,
        // number of solutions found during search (not counting initial BSSF estimate)</returns>
        // For an example of what to return, see DefaultSolver.solve() method.
        public string[] solve()
        {
            throw new NotImplementedException();
        }
    }
}
