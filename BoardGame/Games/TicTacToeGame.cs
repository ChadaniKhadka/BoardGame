using BoardGame.Core;

namespace BoardGame.Games;

// 3x3 board, three in a row wins
public class TicTacToeGame : BaseGame
{
    private const int BoardSize = 3;
    private const int WinLength = 3;

    public override string GameName => "Tic-Tac-Toe";

    public TicTacToeGame(Player[] players) : base(players) { }

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
        "Enter row and column. Example: 2 2";
}
