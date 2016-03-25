using System;
using Newtonsoft.Json;

namespace TSP
{
    static class SolverHelper
    {
        public const double SPACE_PLACEHOLDER = double.PositiveInfinity;

        // Example: https://gist.github.com/TangChr/4711193c87f40c9fb18d
        public static T clone<T>(T source)
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }

        // Populate the state matrix with distances between cities
        public static void initializeState(State state, Problem cityData)
        {
            for (int i = 0; i < cityData.Size; i++)
            {
                City city = cityData.Cities[i];
                for (int j = 0; j < cityData.Size; j++)
                {
                    if (i == j)
                        state.matrix[i, j] = SPACE_PLACEHOLDER;
                    else
                        state.matrix[i, j] = city.costToGetTo(cityData.Cities[j]);
                }
            }

            reduce(state);
        }

        // Makes sure that there is a 0 in every row and column
        public static void reduce(State state)
        {
            // Reduce the rows
            for (int i = 0; i < state.matrix.GetLength(0); i++)
            {
                // Find the smallest value in the row
                double minInRow = double.PositiveInfinity;
                for (int j = 0; j < state.matrix.GetLength(1); j++)
                    minInRow = Math.Min(minInRow, state.matrix[i, j]);

                // If the smallest value in the row is 0,
                // then there already is a 0 in the row
                // We don't need to add a 0 into the row
                //
                // If the smallest value in the row is infinity,
                // then we have a row of infinities
                // Nothing happens
                if (minInRow != 0 && minInRow != SPACE_PLACEHOLDER)
                {
                    // Add the smallest value in the row to the lower bound
                    state.lowerBound += minInRow;

                    // Subtract that value from all the values in the row
                    // The [i,j] that contained the smallest value will be
                    // set to 0, bc minInRow - minInRow = 0;
                    // If the value at [i,j] is the placeholder, nothing happens
                    for (int j = 0; j < state.matrix.GetLength(1); j++)
                        if (state.matrix[i, j] != SPACE_PLACEHOLDER)
                            state.matrix[i, j] -= minInRow;
                }

            }

            // Reduce the columns
            for (int j = 0; j < state.matrix.GetLength(1); j++)
            {
                // Find the smallest value in the column
                double minInColumn = double.PositiveInfinity;
                for (int i = 0; i < state.matrix.GetLength(0); i++)
                    minInColumn = Math.Min(minInColumn, state.matrix[i, j]);

                // Same as above, except switching rows for columns
                if (minInColumn != 0 && minInColumn != SPACE_PLACEHOLDER)
                {
                    // Add the smallest value in the column to the lower bound
                    state.lowerBound += minInColumn;

                    // Subtract that value from all the values in the column
                    for (int i = 0; i < state.matrix.GetLength(0); i++)
                        if (state.matrix[i, j] != SPACE_PLACEHOLDER)
                            state.matrix[i, j] -= minInColumn;
                }
            }
        }
    }
}
