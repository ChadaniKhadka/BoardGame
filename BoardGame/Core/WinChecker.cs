using BoardGame.Games;

namespace BoardGame.Core;

public static class WinChecker
{
    public static bool HasLine(GridBoard board, char symbol, int target)
    {
        int rows = board.Rows;
        int cols = board.Cols;
        char[,] cells = board.Cells;

        if (HasLineInRows(cells, rows, cols, symbol, target))
            return true;
        if (HasLineInColumns(cells, rows, cols, symbol, target))
            return true;
        if (HasLineInDiagonals(cells, rows, cols, symbol, target))
            return true;
        if (HasLineInAntiDiagonals(cells, rows, cols, symbol, target))
            return true;

        return false;
    }

    private static bool HasLineInRows(char[,] cells, int rows, int cols, char symbol, int target)
    {
        for (int r = 0; r < rows; r++)
        {
            int count = 0;
            for (int c = 0; c < cols; c++)
                count = cells[r, c] == symbol ? count + 1 : 0;
            if (count >= target) return true;
        }
        return false;
    }

    private static bool HasLineInColumns(char[,] cells, int rows, int cols, char symbol, int target)
    {
        for (int c = 0; c < cols; c++)
        {
            int count = 0;
            for (int r = 0; r < rows; r++)
                count = cells[r, c] == symbol ? count + 1 : 0;
            if (count >= target) return true;
        }
        return false;
    }

    private static bool HasLineInDiagonals(char[,] cells, int rows, int cols, char symbol, int target)
    {
        for (int r = 0; r <= rows - target; r++)
        {
            for (int c = 0; c <= cols - target; c++)
            {
                int count = 0;
                for (int i = 0; i < target; i++)
                    if (cells[r + i, c + i] == symbol) count++;
                if (count == target) return true;
            }
        }
        return false;
    }

    private static bool HasLineInAntiDiagonals(char[,] cells, int rows, int cols, char symbol, int target)
    {
        for (int r = 0; r <= rows - target; r++)
        {
            for (int c = target - 1; c < cols; c++)
            {
                int count = 0;
                for (int i = 0; i < target; i++)
                    if (cells[r + i, c - i] == symbol) count++;
                if (count == target) return true;
            }
        }
        return false;
    }
}
