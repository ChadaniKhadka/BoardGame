using BoardGame.Games;

namespace BoardGame.Core;

// SRP: only checks win conditions on a GridBoard — no game state knowledge
// DRY: all 4 direction-based games reuse this instead of repeating the logic
public static class WinChecker
{
    // Returns true if 'symbol' has 'target' in a row/col/diagonal on 'board'
    public static bool HasLine(GridBoard board, char symbol, int target)
    {
        int R = board.Rows, C = board.Cols;
        var cells = board.Cells;

        // Rows
        for (int r = 0; r < R; r++)
        {
            int count = 0;
            for (int c = 0; c < C; c++)
                count = cells[r, c] == symbol ? count + 1 : 0;
            if (count >= target) return true;
        }

        // Columns
        for (int c = 0; c < C; c++)
        {
            int count = 0;
            for (int r = 0; r < R; r++)
                count = cells[r, c] == symbol ? count + 1 : 0;
            if (count >= target) return true;
        }

        // Diagonals (↘)
        for (int r = 0; r <= R - target; r++)
            for (int c = 0; c <= C - target; c++)
            {
                int count = 0;
                for (int i = 0; i < target; i++)
                    if (cells[r + i, c + i] == symbol) count++;
                if (count == target) return true;
            }

        // Anti-diagonals (↙)
        for (int r = 0; r <= R - target; r++)
            for (int c = target - 1; c < C; c++)
            {
                int count = 0;
                for (int i = 0; i < target; i++)
                    if (cells[r + i, c - i] == symbol) count++;
                if (count == target) return true;
            }

        return false;
    }
}
