using BoardGame.Core;

namespace BoardGame.Games;

public class NotaktoGame : BaseGame
{
    private const int BoardSize = 3;
    private const int WinLength = 3;
    private const char Piece = 'X';

    public override string GameName => "Notakto";

    public NotaktoGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(BoardSize, BoardSize);

    protected override void DoMove(Move move)
    {
        ((GridMove)move).Value = Piece;
        base.DoMove(move);
    }

    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, Piece, WinLength);

    protected override void AnnounceResult()
    {
        if (Winner is not null)
        {
            Player loser = Winner;
            Player winner = Players.First(p => p != loser);
            Console.WriteLine($"\n*** {winner.Name} wins! ({loser.Name} completed a line) ***");
        }
        else
        {
            Console.WriteLine("\n*** Draw! ***");
        }
    }

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, Piece, WinLength);

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
                        Value = Piece
                    });
        return moves;
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2
            && int.TryParse(parts[0], out int row)
            && int.TryParse(parts[1], out int col)
            && row >= 1 && row <= BoardSize
            && col >= 1 && col <= BoardSize
            && Grid.IsEmpty(row - 1, col - 1))
        {
            return new GridMove
            {
                PlayerIndex = p.Index,
                Row = row - 1,
                Col = col - 1,
                Value = Piece
            };
        }

        Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
        return null;
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        Console.WriteLine("Both players place X. Completing 3 in a row loses.");
        Console.WriteLine("Enter Your Move [row <space> col (e.g. 1 2)] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    protected override void ShowGameHelp()
    {
        Console.WriteLine("  Both players place X. Completing a 3-in-a-row loses!");
        Console.WriteLine("  Try to force your opponent to complete the line.");
    }
}
