using BoardGame.Core;

namespace BoardGame.Games;

// Numerical Tic-Tac-Toe rules:
public class NumericalTicTacToeGame : BaseGame
{
    // Track which numbers have been used
    private readonly HashSet<int> _used = new();

    public override string GameName => "Numerical Tic-Tac-Toe";

    public NumericalTicTacToeGame(Player[] players) : base(players) { }

    protected override void SetupBoard()
    {
        Board = new GridBoard(3, 3, '.');
        _used.Clear();
    }

    public override void LoadFromState(GameState s)
    {
        base.LoadFromState(s);
        RebuildUsed();
    }

    protected override void OnBoardRestored() => RebuildUsed();

    private void RebuildUsed()
    {
        _used.Clear();
        for (int r = 0; r < Grid.Rows; r++)
            for (int c = 0; c < Grid.Cols; c++)
                if (Grid.Cells[r, c] != '.' && int.TryParse(
                    Grid.Cells[r, c].ToString(), out int n))
                    _used.Add(n);
    }

    private bool IsOdd(int playerIdx) => playerIdx == 0;

    private IEnumerable<int> AvailableNumbers(int playerIdx)
    {
        var pool = IsOdd(playerIdx)
            ? new[] { 1, 3, 5, 7, 9 }
            : new[] { 2, 4, 6, 8 };
        return pool.Where(n => !_used.Contains(n));
    }

    // Win: any row, col, or diagonal sums to 15
    protected override bool CheckWin() => LineOf15Exists(Grid);

    public override bool CheckWinOnBoard(Board b) => LineOf15Exists((GridBoard)b);

    private static bool LineOf15Exists(GridBoard g)
    {
        int[,] vals = new int[3, 3];
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
                vals[r, c] = char.IsDigit(g.Cells[r, c]) ? (g.Cells[r, c] - '0') : 0;

        // Rows and cols
        for (int i = 0; i < 3; i++)
        {
            if (vals[i, 0] + vals[i, 1] + vals[i, 2] == 15 &&
                vals[i, 0] != 0 && vals[i, 1] != 0 && vals[i, 2] != 0) return true;
            if (vals[0, i] + vals[1, i] + vals[2, i] == 15 &&
                vals[0, i] != 0 && vals[1, i] != 0 && vals[2, i] != 0) return true;
        }
        // Diagonals
        if (vals[0, 0] + vals[1, 1] + vals[2, 2] == 15 &&
            vals[0, 0] != 0 && vals[1, 1] != 0 && vals[2, 2] != 0) return true;
        if (vals[0, 2] + vals[1, 1] + vals[2, 0] == 15 &&
            vals[0, 2] != 0 && vals[1, 1] != 0 && vals[2, 0] != 0) return true;

        return false;
    }

    protected override bool HasMoves()
        => !Grid.IsFull() && AvailableNumbers(CurrentIdx).Any();

    public override List<Move> GetValidMoves()
    {
        var moves = new List<Move>();
        foreach (var n in AvailableNumbers(CurrentIdx))
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (Grid.IsEmpty(r, c))
                        moves.Add(new GridMove
                        {
                            PlayerIndex = CurrentIdx,
                            Row = r, Col = c,
                            Value = (char)('0' + n)
                        });
        return moves;
    }

    // Override DoMove to track used numbers
    protected override void DoMove(Move move)
    {
        var m = (GridMove)move;
        _used.Add(m.Value - '0');
        base.DoMove(move);
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        var available = AvailableNumbers(currentPlayer.Index).ToList();

        Console.WriteLine($"Your numbers: {string.Join(", ", available)}");
        Console.WriteLine("Enter Your Move [number <space> row <space> col (e.g. 5 2 3)] or Commands:  U=Undo  R=Redo  S=Save  H=Help");
    }

    // Override PromptHumanMove to ask for number + cell
    public override Move? PromptHumanMove(Player p, string input)
    {
        var available = AvailableNumbers(p.Index).ToList();

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 3
            && int.TryParse(parts[0], out int num)
            && int.TryParse(parts[1], out int row)
            && int.TryParse(parts[2], out int col)
            && available.Contains(num)
            && row >= 1 && row <= 3
            && col >= 1 && col <= 3
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

    protected override void ShowGameHelp()
    {
        Console.WriteLine("  Player 1 uses odd numbers  (1 3 5 7 9)");
        Console.WriteLine("  Player 2 uses even numbers (2 4 6 8)");
        Console.WriteLine("  First to get any line summing to 15 wins!");
        Console.WriteLine("  Each number can only be used once.");
        Console.WriteLine("  Input: number row col  (e.g. '5 2 3')");
    }
}
