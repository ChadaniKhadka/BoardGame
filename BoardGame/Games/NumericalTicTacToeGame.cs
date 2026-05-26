using BoardGame.Core;

namespace BoardGame.Games;

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

    protected override void OnBoardRestored() => RebuildUsed();

    private void RebuildUsed()
    {
        _used.Clear();
        for (int r = 0; r < Grid.Rows; r++)
            for (int c = 0; c < Grid.Cols; c++)
                if (Grid.Cells[r, c] != '.' && int.TryParse(
                    Grid.Cells[r, c].ToString(), out int number))
                    _used.Add(number);
    }

    private static bool IsOdd(int playerIdx) => playerIdx == 0;

    private IEnumerable<int> AvailableNumbers(int playerIdx)
    {
        int[] pool = IsOdd(playerIdx) ? OddNumbers : EvenNumbers;
        return pool.Where(n => !_used.Contains(n));
    }

    protected override bool CheckWin() => LineOf15Exists(Grid);

    public override bool CheckWinOnBoard(Board b) => LineOf15Exists((GridBoard)b);

    private static bool LineOf15Exists(GridBoard grid)
    {
        int[,] values = BuildValueGrid(grid);
        return HasWinningRowOrColumn(values) || HasWinningDiagonal(values);
    }

    private static int[,] BuildValueGrid(GridBoard grid)
    {
        int[,] values = new int[BoardSize, BoardSize];
        for (int r = 0; r < BoardSize; r++)
            for (int c = 0; c < BoardSize; c++)
                values[r, c] = char.IsDigit(grid.Cells[r, c]) ? (grid.Cells[r, c] - '0') : 0;
        return values;
    }

    private static bool HasWinningRowOrColumn(int[,] values)
    {
        for (int i = 0; i < BoardSize; i++)
        {
            if (LineSumsTo15(values[i, 0], values[i, 1], values[i, 2])) return true;
            if (LineSumsTo15(values[0, i], values[1, i], values[2, i])) return true;
        }
        return false;
    }

    private static bool HasWinningDiagonal(int[,] values)
    {
        if (LineSumsTo15(values[0, 0], values[1, 1], values[2, 2])) return true;
        if (LineSumsTo15(values[0, 2], values[1, 1], values[2, 0])) return true;
        return false;
    }

    private static bool LineSumsTo15(int a, int b, int c) =>
        a + b + c == WinningLineSum && a != 0 && b != 0 && c != 0;

    protected override bool HasMoves()
        => !Grid.IsFull() && AvailableNumbers(CurrentIdx).Any();

    public override List<Move> GetValidMoves()
    {
        List<Move> moves = new List<Move>();
        foreach (int number in AvailableNumbers(CurrentIdx))
            for (int r = 0; r < BoardSize; r++)
                for (int c = 0; c < BoardSize; c++)
                    if (Grid.IsEmpty(r, c))
                        moves.Add(new GridMove
                        {
                            PlayerIndex = CurrentIdx,
                            Row = r,
                            Col = c,
                            Value = (char)('0' + number)
                        });
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
        List<int> available = AvailableNumbers(currentPlayer.Index).ToList();
        Console.WriteLine($"Your numbers: {string.Join(", ", available)}");
        Console.WriteLine("Enter Your Move [number <space> row <space> col (e.g. 5 2 3)] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        List<int> available = AvailableNumbers(p.Index).ToList();
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

    protected override void ShowGameHelp()
    {
        Console.WriteLine("  Player 1 uses odd numbers  (1 3 5 7 9)");
        Console.WriteLine("  Player 2 uses even numbers (2 4 6 8)");
        Console.WriteLine("  First to get any line summing to 15 wins!");
        Console.WriteLine("  Each number can only be used once.");
        Console.WriteLine("  Input: number row col  (e.g. '5 2 3')");
    }
}
