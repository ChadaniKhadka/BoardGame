using BoardGame.Games;

namespace BoardGame.Core;

// check if a symbol has enough in a row
public static class WinChecker
{
    // see if symbol has a winning line
    public static bool HasLine(GridBoard board, char symbol, int target)
    {
        int rows = board.Rows;
        int cols = board.Cols;
        char[,] cells = board.Cells;

        // look for matching symbols in each row
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

        // look for matching symbols in each column
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

        // check diagonal lines going down right
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

        // check diagonal lines going down left
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
