using BoardGame.Core;

namespace BoardGame.Games;

// Notakto: both players use 'X', completing 3 in a row loses
public class NotaktoGame : BaseGame
{
    public override string GameName => "Notakto";

    public NotaktoGame(Player[] players) : base(players) { }

    protected override void SetupBoard() => Board = new GridBoard(3, 3);

    // In Notakto both players place 'X'
    protected override void DoMove(Move move)
    {
        ((GridMove)move).Value = 'X';
        base.DoMove(move);
    }

    // The player who COMPLETES a line loses
    protected override bool CheckWin()
        => WinChecker.HasLine(Grid, 'X', 3);

    // When CheckWin is true the CURRENT player loses — so the OTHER player wins
    protected override void AnnounceResult()
    {
        if (Winner != null)
        {
            // Winner here is the one who completed the line — they actually lose
            var loser  = Winner;
            var winner = Players.First(p => p != loser);
            Console.WriteLine($"\n*** {winner.Name} wins! ({loser.Name} completed a line) ***");
        }
        else
        {
            Console.WriteLine("\n*** Draw! ***");
        }
    }

    protected override bool HasMoves() => !Grid.IsFull();

    public override bool CheckWinOnBoard(Board b)
        => WinChecker.HasLine((GridBoard)b, 'X', 3);

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
                        Value = 'X'
                    });
        return moves;
    }

  
    // Validate row and column input. Both players place X.
    public override Move? PromptHumanMove(Player p, string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2
            && int.TryParse(parts[0], out int row)
            && int.TryParse(parts[1], out int col)
            && row >= 1 && row <= 3
            && col >= 1 && col <= 3
            && Grid.IsEmpty(row - 1, col - 1))
        {
            return new GridMove
            {
                PlayerIndex = p.Index,
                Row = row - 1,
                Col = col - 1,
                Value = 'X'
            };
        }

        Console.WriteLine("Invalid or occupied cell — enter row and column. Example: 1 2");
        return null;
    }

    public override void ShowTurnInfo(Player currentPlayer)
    {
        Console.WriteLine("Both players place X. Completing 3 in a row loses.");
        Console.WriteLine("Enter Your Move [row <space> col (e.g. 1 2)] or Commands:  U=Undo  R=Redo  S=Save  H=Help");
    }

    protected override void ShowGameHelp()
    {
        Console.WriteLine("  Both players place X. Completing a 3-in-a-row loses!");
        Console.WriteLine("  Try to force your opponent to complete the line.");
    }
}
