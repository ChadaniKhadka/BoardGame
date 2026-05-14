using BoardGame.Core;

namespace BoardGame.Games;

public class GomokuGame : BaseGame
{
    public override string GameName => "Gomoku";

    public GomokuGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(15, 15);

    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, Current.Symbol, 5);

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, Current.Symbol, 5);

    public override List<Move> GetValidMoves()
    {
        var moves = new List<Move>();
        for (int r = 0; r < Grid.Rows; r++)
            for (int c = 0; c < Grid.Cols; c++)
                if (Grid.IsEmpty(r, c))
                    moves.Add(new GridMove
                    {
                        PlayerIndex = CurrentIdx,
                        Row = r, Col = c,
                        Value = Current.Symbol
                    });
        return moves;
    }

    protected override void ShowGameHelp()
        => Console.WriteLine("  Enter row and column 1-15 (e.g. '8 8')");
}
