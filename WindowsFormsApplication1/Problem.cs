using System;
using System.Collections;

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
        private ArrayList _route;
        private TSPSolution _bssf;

        // These three constants are used for convenience/clarity in populating and 
        // accessing the results array that is passed back to the calling form
        public const int COST_POSITION = 0;           
        public const int TIME_POSITION = 1;
        public const int COUNT_POSITION = 2;

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

        // Getters
        public TSPSolution BSSF
        {
            get { return _bssf; }
            set { _bssf = value; }
        }

        public City[] Cities
        {
            get { return _cities; }
        }

        public ArrayList Route
        {
            get { return _route; }
            set { _route = value; }
        }

        public int Seed
        {
            get { return _seed; }
        }

        public int Size
        {
            get { return _size; }
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

        //  Return the cost of the best solution so far. 
        public double costOfBssf ()
        {
            if (_bssf != null)
                return (_bssf.costOfRoute());
            else
                return -1D; 
        }
    }
}
