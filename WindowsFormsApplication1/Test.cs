using System;

namespace TSP
{
    class Test
    {
        public static void run()
        {
            bBHardLastCityBlocked();
            greedyHard();
            bbHardBetterThanGreedy();
        }

        public static void greedyEasy()
        {
            Problem cityData = new Problem(1, 5, 60, HardMode.Modes.Easy);
            cityData = GreedySolver.solve(cityData);
            if (cityData.bssf.costOfRoute != 2060)
                throw new SystemException("Incorrect cost");
        }

        public static void greedyEasy2()
        {
            Problem cityData = new Problem(1, 20, 60, HardMode.Modes.Hard);
            cityData = GreedySolver.solve(cityData);
            if (cityData.bssf.costOfRoute != 4484)
                throw new SystemException("Incorrect cost");
        }

         //This case should return the greedy value bc that is best
        public static void bBEasy()
        {
            Problem cityData = new Problem(1, 5, 60, HardMode.Modes.Easy);
            BranchAndBoundSolver solver = new BranchAndBoundSolver(cityData);
            cityData = solver.solve();
            if (cityData.bssf.costOfRoute != 2060)
                throw new SystemException("Incorrect cost");
        }

        // There exists a better path than the greedy one
        public static void bBNormal()
        {
            Problem cityData = new Problem(268, 16, 60, HardMode.Modes.Normal);
            BranchAndBoundSolver solver = new BranchAndBoundSolver(cityData);
            cityData = solver.solve();
            if (cityData.bssf.costOfRoute != 4069)
                throw new SystemException("Incorrect cost");
        }

        // The last city is blocked
        // Ends up using the greedy value
        public static void bBHardLastCityBlocked()
        {
            Problem cityData = new Problem(1, 5, 60, HardMode.Modes.Hard);
            BranchAndBoundSolver solver = new BranchAndBoundSolver(cityData);
            cityData = solver.solve();
            if (cityData.bssf.costOfRoute != 2123)
                throw new SystemException("Incorrect cost");
        }

        public static void greedyHard()
        {
            Problem cityData = new Problem(1, 20, 60, HardMode.Modes.Hard);
            cityData = GreedySolver.solve(cityData);
            if (cityData.bssf.costOfRoute != 4928)
                throw new SystemException("Incorrect cost");
        }

        public static void bbHardBetterThanGreedy()
        {
            Problem cityData = new Problem(1, 8, 60, HardMode.Modes.Hard);
            BranchAndBoundSolver solver = new BranchAndBoundSolver(cityData);
            cityData = solver.solve();
            if (cityData.bssf.costOfRoute != 3015)
                throw new SystemException("Incorrect cost");
        }
    }
}
