using BoardGame.Core;

namespace BoardGame.Games;

// OCP: adds only what's unique — board size and win length
// LSP: fully substitutable for Game wherever Game is expected
public class TicTacToeGame : BaseGame
{
    public override string GameName => "Tic-Tac-Toe";

    public TicTacToeGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(3, 3);

    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, Current.Symbol, 3);

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, Current.Symbol, 3);

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
        => Console.WriteLine("  Enter row and column 1-3 (e.g. '2 3')");
}
