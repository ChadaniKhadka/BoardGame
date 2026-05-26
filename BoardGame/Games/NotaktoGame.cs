using BoardGame.Core;

namespace BoardGame.Games;

// Notakto: both players place X; the player who completes a line of three loses.
public class NotaktoGame : BaseGame
{
    private const int BoardSize = 3;
    private const int WinLength = 3;
    private const char Piece = 'X';

    public override string GameName => "Notakto";

    public NotaktoGame(Player[] players) : base(players) { }

    protected override void SetupBoard()
    {
        Board = new GridBoard(BoardSize, BoardSize);
    }

    protected override void DoMove(Move move)
    {
        ((GridMove)move).Value = Piece;
        base.DoMove(move);
        if (GameOver && Winner is not null)
            Winner = Players[1 - CurrentIdx];
    }

    protected override bool CheckWin()
    {
        return WinChecker.HasLine(Grid, Piece, WinLength);
    }

    protected override bool HasMoves()
    {
        return !Grid.IsFull();
    }

    public override bool CheckWinOnBoard(Board b)
    {
        return WinChecker.HasLine((GridBoard)b, Piece, WinLength);
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
                        Value = Piece
                    });
                }
            }
        }
        return moves;
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
            return null;
        }

        if (!int.TryParse(parts[0], out int row))
        {
            Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
            return null;
        }

        if (!int.TryParse(parts[1], out int col))
        {
            Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
            return null;
        }

        if (row < 1 || row > BoardSize || col < 1 || col > BoardSize)
        {
            Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
            return null;
        }

        if (!Grid.IsEmpty(row - 1, col - 1))
        {
            Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
            return null;
        }

        return new GridMove
        {
            PlayerIndex = p.Index,
            Row = row - 1,
            Col = col - 1,
            Value = Piece
        };
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        Console.WriteLine("Both players place X. Completing 3 in a row loses.");
        Console.WriteLine("Enter Your Move [row <space> col (e.g. 1 2)] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    protected override string GetGameHelp() =>
        "Enter row and column to place X. Example: 1 3";
}
