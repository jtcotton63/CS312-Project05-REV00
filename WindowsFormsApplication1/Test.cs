using System;

namespace TSP
{
    class Test
    {
        public static void run()
        {
            greedyEasy();
            greedyEasy2();
            bBEasy();
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

        // This case should return the greedy value bc that is best
        public static void bBEasy()
        {
            Problem cityData = new Problem(1, 5, 60, HardMode.Modes.Easy);
            cityData = GreedySolver.solve(cityData);
            if (cityData.bssf.costOfRoute != 2060)
                throw new SystemException("Incorrect cost");
        }
    }
}
