using BoardGame.Core;

namespace BoardGame.Games;

public class TicTacToeGame : BaseGame
{
    private const int BoardSize = 3;
    private const int WinLength = 3;

    public override string GameName => "Tic-Tac-Toe";

    public TicTacToeGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(BoardSize, BoardSize);

    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, Current.Symbol, WinLength);

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, Current.Symbol, WinLength);

    public override List<Move> GetValidMoves()
    {
        List<Move> moves = new List<Move>();
        for (int r = 0; r < Grid.Rows; r++)
            for (int c = 0; c < Grid.Cols; c++)
                if (Grid.IsEmpty(r, c))
                    moves.Add(new GridMove
                    {
                        PlayerIndex = CurrentIdx,
                        Row = r,
                        Col = c,
                        Value = Current.Symbol
                    });
        return moves;
    }

    protected override void ShowGameHelp()
        => Console.WriteLine("  Enter row and column 1-3 (e.g. '2 3')");
}
