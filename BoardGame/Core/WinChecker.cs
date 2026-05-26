using BoardGame.Games;

namespace BoardGame.Core;

// Checks whether a symbol forms a line of the required length on a grid board.
public static class WinChecker
{
    // Return true if symbol appears in a row, column, or diagonal of length target.
    public static bool HasLine(GridBoard board, char symbol, int target)
    {
        int rows = board.Rows;
        int cols = board.Cols;
        char[,] cells = board.Cells;

        // Check every row for a run of matching symbols
        for (int row = 0; row < rows; row++)
        {
            int count = 0;
            for (int col = 0; col < cols; col++)
            {
                if (cells[row, col] == symbol)
                    count++;
                else
                    count = 0;

                if (count >= target)
                    return true;
            }
        }

        // Check every column for a run of matching symbols
        for (int col = 0; col < cols; col++)
        {
            int count = 0;
            for (int row = 0; row < rows; row++)
            {
                if (cells[row, col] == symbol)
                    count++;
                else
                    count = 0;

                if (count >= target)
                    return true;
            }
        }

        // Check diagonals that slope down to the right
        for (int startRow = 0; startRow <= rows - target; startRow++)
        {
            for (int startCol = 0; startCol <= cols - target; startCol++)
            {
                int count = 0;
                for (int step = 0; step < target; step++)
                {
                    if (cells[startRow + step, startCol + step] == symbol)
                        count++;
                }
                if (count == target)
                    return true;
            }
        }

        // Check diagonals that slope down to the left
        for (int startRow = 0; startRow <= rows - target; startRow++)
        {
            for (int startCol = target - 1; startCol < cols; startCol++)
            {
                int count = 0;
                for (int step = 0; step < target; step++)
                {
                    if (cells[startRow + step, startCol - step] == symbol)
                        count++;
                }
                if (count == target)
                    return true;
            }
        }

        return false;
    }
}
