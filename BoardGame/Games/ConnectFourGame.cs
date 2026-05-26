using BoardGame.Core;

namespace BoardGame.Games;

// Connect Four on a 6x7 board: pieces drop to the bottom of a column.
public class ConnectFourGame : BaseGame
{
    private const int BoardRows = 6;
    private const int BoardCols = 7;
    private const int WinLength = 4;
    private const int NoRow = -1;

    public override string GameName => "Connect Four";

    public ConnectFourGame(Player[] players) : base(players) { }

    protected override void SetupBoard()
    {
        Board = new GridBoard(BoardRows, BoardCols);
    }

    // Find the lowest empty row in the given column
    private int DropRow(int col)
    {
        for (int r = Grid.Rows - 1; r >= 0; r--)
        {
            if (Grid.IsEmpty(r, col))
                return r;
        }
        return NoRow;
    }

    protected override bool CheckWin()
    {
        return WinChecker.HasLine(Grid, Current.Symbol, WinLength);
    }

    protected override bool HasMoves()
    {
        return !Grid.IsFull();
    }

    public override bool CheckWinOnBoard(Board b)
    {
        return WinChecker.HasLine((GridBoard)b, Current.Symbol, WinLength);
    }

    public override List<Move> GetValidMoves()
    {
        List<Move> moves = new List<Move>();
        for (int c = 0; c < Grid.Cols; c++)
        {
            int row = DropRow(c);
            if (row >= 0)
            {
                moves.Add(new GridMove
                {
                    PlayerIndex = CurrentIdx,
                    Row = row,
                    Col = c,
                    Value = Current.Symbol
                });
            }
        }
        return moves;
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        Console.WriteLine($"Enter Your Move [column from 1-{Grid.Cols}] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        if (!int.TryParse(input, out int col))
        {
            Console.WriteLine($"Column full or invalid — enter a column number from 1 to {Grid.Cols}.");
            return null;
        }

        if (col < 1 || col > Grid.Cols)
        {
            Console.WriteLine($"Column full or invalid — enter a column number from 1 to {Grid.Cols}.");
            return null;
        }

        int row = DropRow(col - 1);
        if (row < 0)
        {
            Console.WriteLine($"Column full or invalid — enter a column number from 1 to {Grid.Cols}.");
            return null;
        }

        return new GridMove
        {
            PlayerIndex = p.Index,
            Row = row,
            Col = col - 1,
            Value = p.Symbol
        };
    }

    protected override string GetGameHelp() =>
        "Enter a column number (1-7). Example: 4";
}
