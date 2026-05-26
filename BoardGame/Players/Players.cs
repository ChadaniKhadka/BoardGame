using BoardGame.Core;

namespace BoardGame.Players;

public class HumanPlayer : Player
{
    public HumanPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game) => null;
}

public class ComputerPlayer : Player
{
    private readonly Random _rng = new();

    public ComputerPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game)
    {
        Console.WriteLine($"  {Name} is thinking...");
        Thread.Sleep(400);

        List<Move> moves = game.GetValidMoves();

        foreach (Move candidate in moves)
        {
            Board clone = board.Clone();
            clone.ApplyMove(candidate);
            if (game.CheckWinOnBoard(clone)) return candidate;
        }

        return moves[_rng.Next(moves.Count)];
    }
}
