using BoardGame.Core;

namespace BoardGame.Players;

// human types moves in the console
public class HumanPlayer : Player
{
    public HumanPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game)
    {
        return null;
    }
}

// computer tries to win or picks at random
public class ComputerPlayer : Player
{
    private readonly Random _rng = new();

    public ComputerPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game)
    {
        List<Move> moves = game.GetValidMoves();

        foreach (Move candidate in moves)
        {
            Board clone = board.Clone();
            clone.ApplyMove(candidate);
            if (game.CheckWinOnBoard(clone))
                return candidate;
        }

        if (moves.Count == 0)
            return null;

        int index = _rng.Next(moves.Count);
        return moves[index];
    }
}
