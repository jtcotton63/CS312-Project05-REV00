using System;

namespace TSP
{
    class Test
    {
        public static void run()
        {
            greedyEasy();
        }

        public static void greedyEasy()
        {
            Problem cityData = new Problem(1, 5, 60, HardMode.Modes.Easy);
            string[] results = GreedySolver.solve(cityData);
            if (!results[Problem.COST_POSITION].Equals("2060"))
                throw new SystemException("Incorrect cost");
        }
    }
}
