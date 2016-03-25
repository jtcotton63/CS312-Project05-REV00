using System;
using System.Collections;
using System.Collections.Generic;

namespace TSP
{
    class Problem
    {
        // Default values
        public const int DEFAULT_SEED = 1;
        public const int DEFAULT_PROBLEM_SIZE = 20;
        public const int DEFAULT_TIME_LIMIT = 60;        // in seconds
        private const double FRACTION_OF_PATHS_TO_REMOVE = 0.20;

        // Class members
        private int _seed;
        private int _size;
        private int _timeLimit;
        private HardMode.Modes _mode;
        private Random _rnd;

        private City[] _cities;
        private TSPSolution _bssf;
        private int numSolutions;

        // Constructors
        public Problem()
        {
            initialize(DEFAULT_SEED, DEFAULT_PROBLEM_SIZE, DEFAULT_TIME_LIMIT, HardMode.Modes.Hard);
        }

        public Problem(int seed, int size, int timeInSeconds, HardMode.Modes mode)
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
        }

        // Getters and setters
        public TSPSolution BSSF
        {
            get { return _bssf; }
            set { _bssf = value; }
        }

        public City[] Cities
        {
            get { return _cities; }
        }

        public int Seed
        {
            get { return _seed; }
        }

        public int Size
        {
            get { return _size; }
        }

        public int Solutions
        {
            get { return numSolutions; }
            set { numSolutions = value; }
        }

        // Reset the problem instance
        private void resetData()
        {
            _cities = new City[_size];
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

        //  Return the cost of the best solution so far. 
        public double costOfBssf()
        {
            if (_bssf != null)
            {
                // go through each edge in the route and add up the cost. 
                int x;
                List<City> route = _bssf.route;
                City here;
                double cost = 0D;

                for (x = 0; x < route.Count - 1; x++)
                {
                    here = route[x];
                    cost += here.costToGetTo(route[x + 1]);
                }

                // go from the last city to the first. 
                here = route[route.Count - 1];
                cost += here.costToGetTo(route[0]);
                return cost;
            }
            else
                return -1D; 
        }
    }
}
