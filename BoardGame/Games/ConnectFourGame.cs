using BoardGame.Core;

namespace BoardGame.Games;

// SRP: ConnectFourGame only adds gravity logic and column-based input
public class ConnectFourGame : BaseGame
{
    public override string GameName => "Connect Four";

    public ConnectFourGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(6, 7);

    // Gravity: find the lowest empty row in a column
    private int DropRow(int col)
    {
        for (int r = Grid.Rows - 1; r >= 0; r--)
            if (Grid.IsEmpty(r, col)) return r;
        return -1;
    }

    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, Current.Symbol, 4);

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, Current.Symbol, 4);

    public override List<Move> GetValidMoves()
    {
        var moves = new List<Move>();
        for (int c = 0; c < Grid.Cols; c++)
        {
            int r = DropRow(c);
            if (r >= 0)
                moves.Add(new GridMove
                {
                    PlayerIndex = CurrentIdx,
                    Row = r, Col = c,
                    Value = Current.Symbol
                });
        }
        return moves;
    }

    // Override: column-only input, gravity determines row
    public override Move PromptHumanMove(Player p)
    {
        while (true)
        {
            Console.Write($"  Enter column (1-{Grid.Cols}): ");
            string? line = Console.ReadLine()?.Trim();

            if (int.TryParse(line, out int col)
                && col >= 1 && col <= Grid.Cols)
            {
                int r = DropRow(col - 1);
                if (r >= 0)
                    return new GridMove
                    {
                        PlayerIndex = p.Index,
                        Row = r, Col = col - 1,
                        Value = p.Symbol
                    };
            }
            Console.WriteLine("  Column full or invalid — try again.");
        }
    }

    protected override void ShowGameHelp()
        => Console.WriteLine("  Enter a column number 1-7. Pieces fall to the bottom.");
}
