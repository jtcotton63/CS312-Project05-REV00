using System;
using System.Collections.Generic;

namespace TSP
{
    class State
    {
        public int size;
        public int cityOfOriginIndex;
        public int currCityIndex;
        public double[,] matrix;
        public double lowerBound;
        public List<int> cityOrder;


        public State(int size)
        {
            this.size = size;
            this.cityOfOriginIndex = 0;
            this.currCityIndex = 0;
            this.matrix = new double[size, size];
            this.lowerBound = 0;
            this.cityOrder = new List<int>(size+1);
        }

        public void addCity(int cityIndex)
        {
            this.cityOrder.Add(cityIndex);
        }

        public int getCitiesVisited()
        {
            return this.cityOrder.Count;
        }

        public void printState()
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == 0)
                        Console.Write("00000");
                    else if (matrix[i, j] == double.PositiveInfinity)
                        Console.Write("infin");
                    else if (matrix[i, j] < 10)
                    {
                        Console.Write("0000");
                        Console.Write(matrix[i, j]);
                    }
                    else if (matrix[i, j] < 100)
                    {
                        Console.Write("000");
                        Console.Write(matrix[i, j]);
                    }
                    else if (matrix[i, j] < 1000)
                    {
                        Console.Write("00");
                        Console.Write(matrix[i, j]);
                    }
                    else if (matrix[i, j] < 10000)
                    {
                        Console.Write("0");
                        Console.Write(matrix[i, j]);
                    }
                    else
                        Console.Write(matrix[i, j]);

                    Console.Write(',');
                }
                Console.Write("\n");
            }
            Console.Write("\n\n");
        }
    }
}
