using BoardGame.Core;

namespace BoardGame.Players;

// A human player — moves come from console input handled by the game class.
public class HumanPlayer : Player
{
    public HumanPlayer(string name, char symbol, int index)
        : base(name, symbol, index) { }

    public override Move? GetMove(Board board, Game game)
    {
        return null;
    }
}

// A computer player — picks a winning move if possible, otherwise chooses at random.
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
