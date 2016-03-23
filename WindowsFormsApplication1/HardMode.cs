using System;
using System.Collections.Generic;

namespace TSP
{
    // This class is used in "hard" mode, where edges are selectively removed. - APS
    class HardMode
    {
        // Difficulty Modes:
        // Easy:   Distances are symmetric (for debugging)
        // Normal: Distances are asymmetric
        // Hard:   Asymmetric distances; some paths are blocked
        public enum Modes { Easy = 0, Normal, Hard };
        private const Modes DEFAULT_MODE = Modes.Normal;

        // Edge object, used only to keep track of edge removal
        private struct Edge
        {
            public City city1, city2;
            public Edge(City city1, City city2)
            {
                this.city1 = city1;
                this.city2 = city2;
            }
        }

        // Class variables
        private Modes _mode;
        private HashSet<Edge> _removedEdges; // List of edges that are removed
        private City[] _cities; // Keep a local copy of the list of cities
        private Random _rnd; // Local reference to the random number generator, to allow repeatable runs

        public HardMode(Modes mode, Random rnd, City[] cities)
        {
            this._mode = mode;
            this._rnd = rnd;
            this._cities = cities;
            this._removedEdges = new HashSet<Edge>();
        }

        // removes a specified number of paths, 
        // (one-way only, e.g., if we remove a path from A to B, the return path from B to A still exists)
        public void removePaths(int numberToRemove)
        {
            // Make sure we don't remove an impossible number of edges
            // N^2 - N(diagonals) - N (remaining paths) = (N)(N-2)
            int maxPathsToRemove = _cities.Length * (_cities.Length - 2);
            if (numberToRemove > maxPathsToRemove)
                numberToRemove = maxPathsToRemove;

            _removedEdges = new HashSet<Edge>();
            // The reference path ensures that a valid path always remains after our deleting frenzy.
            HashSet<Edge> referencePath = generateReferencePath(_cities);
            for (int i = 0; i < numberToRemove; i++)
            {
                bool removed = false;
                while (!removed)
                {
                    Edge candidateEdge = new Edge(
                        _cities[_rnd.Next(_cities.Length)],
                        _cities[_rnd.Next(_cities.Length)]
                        );
                    if (!candidateEdge.city1.Equals(candidateEdge.city2) &&
                        !referencePath.Contains(candidateEdge) &&
                        !_removedEdges.Contains(candidateEdge))
                    {
                        _removedEdges.Add(candidateEdge);
                        removed = true;
                    }
                }

            }

        }

        // This is an optimization -- we cache one object to avoid repeated memory allocation & deallocation
        // It should only be used in isRemoved and should never be returned to user code, since its internal 
        // values will keep changing.
        static Edge tempEdge = new Edge();

        // this method is not thread-safe, i.e., it should not be called from two separate threads
        public bool isEdgeRemoved(City city1, City city2)
        {
            tempEdge.city1 = city1;
            tempEdge.city2 = city2;
            return _removedEdges.Contains(tempEdge);
        }

        // Shuffles cities to generate a temporary reference path.  The reference path
        // guarantees that we don't create an impossible graph when removing edges.
        private HashSet<Edge> generateReferencePath(City[] cities) //List<City> cities)
        {
            City[] referencePath = new City[cities.Length];
            City[] remainingCities = new City[cities.Length];
            for (int i = 0; i < cities.Length; i++)
                remainingCities[i] = cities[i];
            int remainingSize = remainingCities.Length;
            for (int i = 0; i < referencePath.Length; i++)
            {
                int index = _rnd.Next() % remainingSize;
                referencePath[i] = remainingCities[index];
                remainingCities[index] = remainingCities[remainingSize - 1];
            }
            // put the loop into a HashSet of Edges...
            HashSet<Edge> referenceSet = new HashSet<Edge>();
            for (int i = 0; i < cities.Length - 1; i++)
                referenceSet.Add(new Edge(cities[i], cities[(i + 1) % cities.Length]));
            return referenceSet;
        }

        public static Modes getMode(String modeName)
        {
            string[] ModeNames = { "Easy", "Normal", "Hard" }; // in corresponding order

            for (int i = 0; i < ModeNames.Length; i++)
                if (ModeNames[i].Equals(modeName, StringComparison.OrdinalIgnoreCase))
                    return (Modes)i;
            return DEFAULT_MODE;
        }
    }

 }
