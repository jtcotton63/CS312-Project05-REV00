using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

namespace TSP
{

    class ProblemAndSolver
    {
        // Class members
        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;

        // Default values
        private const int DEFAULT_SEED = 1;
        private const int DEFAULT_PROBLEM_SIZE = 20;
        private const int DEFAULT_TIME_LIMIT = 60;        // in seconds
        private const int CITY_ICON_SIZE = 5;
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        private City[] Cities;
        private ArrayList Route;
        private TSPSolution bssf; 

        private int seed;
        private int size;
        private int time_limit;
        private HardMode.Modes _mode;
        private Random rnd;

        // These three constants are used for convenience/clarity in populating and 
        // accessing the results array that is passed back to the calling form
        public const int COST = 0;           
        public const int TIME = 1;
        public const int COUNT = 2;
        
        public int Size
        {
            get { return size; }
        }

        public int Seed
        {
            get { return seed; }
        }

        // Constructors
        public ProblemAndSolver()
        {
            initialize(DEFAULT_SEED, DEFAULT_PROBLEM_SIZE, DEFAULT_TIME_LIMIT);
        }

        public ProblemAndSolver(int seed)
        {
            initialize(seed, DEFAULT_PROBLEM_SIZE, DEFAULT_TIME_LIMIT);
        }

        public ProblemAndSolver(int seed, int size)
        {
            initialize(seed, size, DEFAULT_TIME_LIMIT);
        }

        public ProblemAndSolver(int seed, int size, int time)
        {
            initialize(seed, size, time);
        }

        private void initialize(int seed, int size, int time)
        {
            this.seed = seed;
            this.size = size;
            rnd = new Random(seed);
            this.time_limit = time * 1000;                        // TIME_LIMIT is in seconds, but timer wants it in milliseconds
            this.resetData();
        }

        // Reset the problem instance
        private void resetData()
        {
            Cities = new City[size];
            Route = new ArrayList(size);
            bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < size; i++)
                    Cities[i] = new City(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this.rnd, Cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);

            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue,1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        // make a new problem with the given size.
        // <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode)
        {
            this.size = size;
            this._mode = mode;
            resetData();
        }

        // make a new problem with the given size, now including timelimit paremeter that was added to form.
        // <param name="size">number of cities</param>
        public void GenerateProblem(int size, HardMode.Modes mode, int timelimit)
        {
            this.size = size;
            this._mode = mode;
            this.time_limit = timelimit*1000;                                   //convert seconds to milliseconds
            resetData();
        }

        // return a copy of the cities in this problem. 
        // <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[Cities.Length];
            Array.Copy(Cities, retCities, Cities.Length);
            return retCities;
        }

        // draw the cities in the problem.  if the bssf member is defined, then
        // draw that too. 
        // <param name="g">where to draw the stuff</param>
        public void Draw(Graphics g)
        {
            float width  = g.VisibleClipBounds.Width-45F;
            float height = g.VisibleClipBounds.Height-45F;
            Font labelFont = new Font("Arial", 10);

            // Draw lines
            if (bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[bssf.Route.Count];
                int index = 0;
                foreach (City c in bssf.Route)
                {
                    if (index < bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)Cities[0].X * width - 1, (float)Cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in Cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        //  Return the cost of the best solution so far. 
        public double costOfBssf ()
        {
            if (bssf != null)
                return (bssf.costOfRoute());
            else
                return -1D; 
        }

        // This is the entry point for the default solver
        // which just finds a valid random tour 
        // <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution, number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] solveDefault()
        {
            int i, swap, temp, count=0;
            string[] results = new string[3];
            int[] perm = new int[Cities.Length];
            Route = new ArrayList();
            Random rnd = new Random();
            Stopwatch timer = new Stopwatch();

            timer.Start();

            do
            {
                for (i = 0; i < perm.Length; i++)                                 // create a random permutation template
                    perm[i] = i;
                for (i = 0; i < perm.Length; i++)
                {
                    swap = i;
                    while (swap == i)
                        swap = rnd.Next(0, Cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                Route.Clear();
                for (i = 0; i < Cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    Route.Add(Cities[perm[i]]);
                }
                bssf = new TSPSolution(Route);
                count++;
            } while (costOfBssf() == double.PositiveInfinity);                // until a valid route is found
            timer.Stop();

            results[COST] = costOfBssf().ToString();                          // load results array
            results[TIME] = timer.Elapsed.ToString();
            results[COUNT] = count.ToString();

            return results;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Project 5
        ////////////////////////////////////////////////////////////////////////////////////////////

        // performs a Branch and Bound search of the state space of partial tours
        // stops when time limit expires and uses BSSF as solution
        // <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution,
        // number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] solveBranchAndBound()
        {
            throw new NotImplementedException();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Project 6
        ////////////////////////////////////////////////////////////////////////////////////////////

        // finds the greedy tour starting from each city and keeps the best (valid) one
        // <returns>results array for GUI that contains three ints: cost of solution, time spent to find solution,
        // number of solutions found during search (not counting initial BSSF estimate)</returns>
        public string[] solveGreedy()
        {
            throw new NotImplementedException();
        }

        public string[] solveFancy()
        {
            throw new NotImplementedException();
        }

    }

}
