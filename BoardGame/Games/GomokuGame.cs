using BoardGame.Core;

namespace BoardGame.Games;

// Gomoku on a 15x15 board: first player to get five in a row wins.
public class GomokuGame : BaseGame
{
    private const int BoardSize = 15;
    private const int WinLength = 5;

    public override string GameName => "Gomoku";

    public GomokuGame(Player[] players) : base(players) { }

    protected override void SetupBoard()
    {
        Board = new GridBoard(BoardSize, BoardSize);
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
        for (int r = 0; r < Grid.Rows; r++)
        {
            for (int c = 0; c < Grid.Cols; c++)
            {
                if (Grid.IsEmpty(r, c))
                {
                    moves.Add(new GridMove
                    {
                        PlayerIndex = CurrentIdx,
                        Row = r,
                        Col = c,
                        Value = Current.Symbol
                    });
                }
            }
        }
        return moves;
    }

    protected override string GetGameHelp() =>
        "Enter row and column. Example: 8 8";
}
