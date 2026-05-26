using BoardGame.Core;

namespace BoardGame.Games;

// Numerical tic-tac-toe: odd/even numbers on a 3x3 grid, first line summing to 15 wins.
public class NumericalTicTacToeGame : BaseGame
{
    private const int BoardSize = 3;
    private const int WinningLineSum = 15;
    private static readonly int[] OddNumbers = { 1, 3, 5, 7, 9 };
    private static readonly int[] EvenNumbers = { 2, 4, 6, 8 };

    private readonly HashSet<int> _used = new();

    public override string GameName => "Numerical Tic-Tac-Toe";

    public NumericalTicTacToeGame(Player[] players) : base(players) { }

    protected override void SetupBoard()
    {
        Board = new GridBoard(BoardSize, BoardSize, '.');
        _used.Clear();
    }

    public override void LoadFromState(GameState s)
    {
        base.LoadFromState(s);
        RebuildUsed();
    }

    protected override void OnBoardRestored()
    {
        RebuildUsed();
    }

    // Rebuild the set of numbers already placed on the board
    private void RebuildUsed()
    {
        _used.Clear();
        for (int r = 0; r < Grid.Rows; r++)
        {
            for (int c = 0; c < Grid.Cols; c++)
            {
                char cell = Grid.Cells[r, c];
                if (cell != '.' && int.TryParse(cell.ToString(), out int number))
                    _used.Add(number);
            }
        }
    }

    private static bool IsOdd(int playerIdx)
    {
        return playerIdx == 0;
    }

    // Return the numbers this player can still place
    private List<int> AvailableNumbers(int playerIdx)
    {
        int[] pool;
        if (IsOdd(playerIdx))
            pool = OddNumbers;
        else
            pool = EvenNumbers;

        List<int> available = new List<int>();
        foreach (int number in pool)
        {
            if (!_used.Contains(number))
                available.Add(number);
        }
        return available;
    }

    protected override bool CheckWin()
    {
        return LineOf15Exists(Grid);
    }

    public override bool CheckWinOnBoard(Board b)
    {
        return LineOf15Exists((GridBoard)b);
    }

    // Return true if any row, column, or diagonal sums to 15 with no empty cells
    private static bool LineOf15Exists(GridBoard grid)
    {
        int[,] values = new int[BoardSize, BoardSize];
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (char.IsDigit(grid.Cells[r, c]))
                    values[r, c] = grid.Cells[r, c] - '0';
                else
                    values[r, c] = 0;
            }
        }

        for (int row = 0; row < BoardSize; row++)
        {
            int sum = values[row, 0] + values[row, 1] + values[row, 2];
            if (sum == WinningLineSum
                && values[row, 0] != 0
                && values[row, 1] != 0
                && values[row, 2] != 0)
                return true;
        }

        for (int col = 0; col < BoardSize; col++)
        {
            int sum = values[0, col] + values[1, col] + values[2, col];
            if (sum == WinningLineSum
                && values[0, col] != 0
                && values[1, col] != 0
                && values[2, col] != 0)
                return true;
        }

        int mainDiagonal = values[0, 0] + values[1, 1] + values[2, 2];
        if (mainDiagonal == WinningLineSum
            && values[0, 0] != 0
            && values[1, 1] != 0
            && values[2, 2] != 0)
            return true;

        int antiDiagonal = values[0, 2] + values[1, 1] + values[2, 0];
        if (antiDiagonal == WinningLineSum
            && values[0, 2] != 0
            && values[1, 1] != 0
            && values[2, 0] != 0)
            return true;

        return false;
    }

    protected override bool HasMoves()
    {
        if (Grid.IsFull())
            return false;

        List<int> available = AvailableNumbers(CurrentIdx);
        return available.Count > 0;
    }

    public override List<Move> GetValidMoves()
    {
        List<Move> moves = new List<Move>();
        List<int> available = AvailableNumbers(CurrentIdx);

        foreach (int number in available)
        {
            for (int r = 0; r < BoardSize; r++)
            {
                for (int c = 0; c < BoardSize; c++)
                {
                    if (Grid.IsEmpty(r, c))
                    {
                        moves.Add(new GridMove
                        {
                            PlayerIndex = CurrentIdx,
                            Row = r,
                            Col = c,
                            Value = (char)('0' + number)
                        });
                    }
                }
            }
        }
        return moves;
    }

    protected override void DoMove(Move move)
    {
        GridMove gridMove = (GridMove)move;
        _used.Add(gridMove.Value - '0');
        base.DoMove(move);
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        List<int> available = AvailableNumbers(currentPlayer.Index);
        Console.WriteLine($"Your numbers: {string.Join(", ", available)}");
        Console.WriteLine("Enter Your Move [number <space> row <space> col (e.g. 5 2 3)] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        List<int> available = AvailableNumbers(p.Index);
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 3
            && int.TryParse(parts[0], out int num)
            && int.TryParse(parts[1], out int row)
            && int.TryParse(parts[2], out int col)
            && available.Contains(num)
            && row >= 1 && row <= BoardSize
            && col >= 1 && col <= BoardSize
            && Grid.IsEmpty(row - 1, col - 1))
        {
            return new GridMove
            {
                PlayerIndex = p.Index,
                Row = row - 1,
                Col = col - 1,
                Value = (char)('0' + num)
            };
        }

        Console.WriteLine("     Invalid — enter available number, row and column. Example: 5 2 3");
        return null;
    }

    protected override string GetGameHelp() =>
        "Enter number, row and column. Example: 5 2 3";
}
