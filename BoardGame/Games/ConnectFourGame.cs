using BoardGame.Core;

namespace BoardGame.Games;

public class ConnectFourGame : BaseGame
{
    public override string GameName => "Connect Four";

    public ConnectFourGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(6, 7);
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
    public override void ShowTurnInfo(Player currentPlayer)
{
    Console.WriteLine($"Enter Your Move [column from 1-{Grid.Cols}] or Commands:  U=Undo  R=Redo  S=Save  H=Help");
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
