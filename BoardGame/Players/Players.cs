using BoardGame.Core;

namespace BoardGame.Players;

// LSP: both players are fully interchangeable wherever Player is expected
// SRP: HumanPlayer only delegates to the game's prompt — no input logic here
public class HumanPlayer : Player
{
    public HumanPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game) => null;
}

// SRP: ComputerPlayer only chooses a move — win-check logic lives in the game
public class ComputerPlayer : Player
{
    private readonly Random _rng = new();

    public ComputerPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game)
    {
        Console.WriteLine($"  {Name} is thinking...");
        Thread.Sleep(400);

        var moves = game.GetValidMoves();

        // Try to win immediately; otherwise pick at random (KISS)
        foreach (var m in moves)
        {
            var clone = board.Clone();
            clone.ApplyMove(m);
            if (game.CheckWinOnBoard(clone)) return m;
        }

        return moves[_rng.Next(moves.Count)];
    }
}
