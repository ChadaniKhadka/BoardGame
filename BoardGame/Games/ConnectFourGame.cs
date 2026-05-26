using BoardGame.Core;

namespace BoardGame.Games;

public class ConnectFourGame : BaseGame
{
    private const int BoardRows = 6;
    private const int BoardCols = 7;
    private const int WinLength = 4;
    private const int NoRow = -1;

    public override string GameName => "Connect Four";

    public ConnectFourGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(BoardRows, BoardCols);

    private int DropRow(int col)
    {
        for (int r = Grid.Rows - 1; r >= 0; r--)
            if (Grid.IsEmpty(r, col)) return r;
        return NoRow;
    }

    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, Current.Symbol, WinLength);

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, Current.Symbol, WinLength);

    public override List<Move> GetValidMoves()
    {
        List<Move> moves = new List<Move>();
        for (int c = 0; c < Grid.Cols; c++)
        {
            int row = DropRow(c);
            if (row >= 0)
                moves.Add(new GridMove
                {
                    PlayerIndex = CurrentIdx,
                    Row = row,
                    Col = c,
                    Value = Current.Symbol
                });
        }
        return moves;
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        Console.WriteLine($"Enter Your Move [column from 1-{Grid.Cols}] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        if (int.TryParse(input, out int col)
            && col >= 1 && col <= Grid.Cols)
        {
            int row = DropRow(col - 1);

            if (row >= 0)
            {
                return new GridMove
                {
                    PlayerIndex = p.Index,
                    Row = row,
                    Col = col - 1,
                    Value = p.Symbol
                };
            }
        }

        Console.WriteLine($"Column full or invalid — enter a column number from 1 to {Grid.Cols}.");
        return null;
    }

    protected override void ShowGameHelp()
        => Console.WriteLine("  Enter a column number 1-7. Pieces fall to the bottom.");
}
