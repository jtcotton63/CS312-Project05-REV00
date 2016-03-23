using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;

namespace TSP
{

    class ProblemAndSolver
    {
        // Default values
        public const int DEFAULT_SEED = 1;
        public const int DEFAULT_PROBLEM_SIZE = 20;
        public const int DEFAULT_TIME_LIMIT = 60;        // in seconds
        private const int CITY_ICON_SIZE = 5;
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;


        // Class members
        private int _seed;
        private int _size;
        private int _timeLimit;
        private HardMode.Modes _mode;
        private Random _rnd;

        private City[] _cities;
        private ArrayList _route;
        private TSPSolution _bssf;

        private Brush cityBrushStartStyle;
        private Brush cityBrushStyle;
        private Pen routePenStyle;

        // These three constants are used for convenience/clarity in populating and 
        // accessing the results array that is passed back to the calling form
        public const int COST = 0;           
        public const int TIME = 1;
        public const int COUNT = 2;
        
        public int Size
        {
            get { return _size; }
        }

        public int Seed
        {
            get { return _seed; }
        }

        // Constructors
        public ProblemAndSolver()
        {
            initialize(DEFAULT_SEED, DEFAULT_PROBLEM_SIZE, DEFAULT_TIME_LIMIT, HardMode.Modes.Hard);
        }

        public ProblemAndSolver(int seed, int size, int timeInSeconds, HardMode.Modes mode)
        {
            initialize(seed, size, timeInSeconds, mode);
        }

        private void initialize(int seed, int problemSize, int timeInSeconds, HardMode.Modes mode)
        {
            this._seed = seed;
            this._rnd = new Random(seed);
            this._size = problemSize;
            this._mode = mode;
            this._timeLimit = timeInSeconds * 1000;                        // timer wants timeLimit in milliseconds
            this.resetData();

            // Take care of the brushes
            cityBrushStyle = new SolidBrush(Color.Black);
            cityBrushStartStyle = new SolidBrush(Color.Red);
            routePenStyle = new Pen(Color.Blue, 1);
            routePenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
        }

        // Reset the problem instance
        private void resetData()
        {
            _cities = new City[_size];
            _route = new ArrayList(_size);
            _bssf = null;

            if (_mode == HardMode.Modes.Easy)
            {
                for (int i = 0; i < _size; i++)
                    _cities[i] = new City(_rnd.NextDouble(), _rnd.NextDouble());
            }
            else // Medium and hard
            {
                for (int i = 0; i < _size; i++)
                    _cities[i] = new City(_rnd.NextDouble(), _rnd.NextDouble(), _rnd.NextDouble() * City.MAX_ELEVATION);
            }

            HardMode mm = new HardMode(this._mode, this._rnd, _cities);
            if (_mode == HardMode.Modes.Hard)
            {
                int edgesToRemove = (int)(_size * FRACTION_OF_PATHS_TO_REMOVE);
                mm.removePaths(edgesToRemove);
            }
            City.setModeManager(mm);
        }

        // return a copy of the cities in this problem. 
        // <returns>array of cities</returns>
        public City[] GetCities()
        {
            City[] retCities = new City[_cities.Length];
            Array.Copy(_cities, retCities, _cities.Length);
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
            if (_bssf != null)
            {
                // make a list of points. 
                Point[] ps = new Point[_bssf.Route.Count];
                int index = 0;
                foreach (City c in _bssf.Route)
                {
                    if (index < _bssf.Route.Count -1)
                        g.DrawString(" " + index +"("+c.costToGetTo(_bssf.Route[index+1]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    else 
                        g.DrawString(" " + index +"("+c.costToGetTo(_bssf.Route[0]as City)+")", labelFont, cityBrushStartStyle, new PointF((float)c.X * width + 3F, (float)c.Y * height));
                    ps[index++] = new Point((int)(c.X * width) + CITY_ICON_SIZE / 2, (int)(c.Y * height) + CITY_ICON_SIZE / 2);
                }

                if (ps.Length > 0)
                {
                    g.DrawLines(routePenStyle, ps);
                    g.FillEllipse(cityBrushStartStyle, (float)_cities[0].X * width - 1, (float)_cities[0].Y * height - 1, CITY_ICON_SIZE + 2, CITY_ICON_SIZE + 2);
                }

                // draw the last line. 
                g.DrawLine(routePenStyle, ps[0], ps[ps.Length - 1]);
            }

            // Draw city dots
            foreach (City c in _cities)
            {
                g.FillEllipse(cityBrushStyle, (float)c.X * width, (float)c.Y * height, CITY_ICON_SIZE, CITY_ICON_SIZE);
            }

        }

        //  Return the cost of the best solution so far. 
        public double costOfBssf ()
        {
            if (_bssf != null)
                return (_bssf.costOfRoute());
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
            int[] perm = new int[_cities.Length];
            _route = new ArrayList();
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
                        swap = rnd.Next(0, _cities.Length);
                    temp = perm[i];
                    perm[i] = perm[swap];
                    perm[swap] = temp;
                }
                _route.Clear();
                for (i = 0; i < _cities.Length; i++)                            // Now build the route using the random permutation 
                {
                    _route.Add(_cities[perm[i]]);
                }
                _bssf = new TSPSolution(_route);
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
