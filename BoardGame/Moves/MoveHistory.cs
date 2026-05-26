using BoardGame.Core;

namespace BoardGame.Moves;

// Stores one move together with a board snapshot taken before the move was applied.
public class RecordedMove
{
    public Move Move { get; }
    public Board BoardSnapshot { get; }
    public int PlayerIndex { get; }

    public RecordedMove(Move move, Board boardSnapshot, int playerIndex)
    {
        Move = move;
        BoardSnapshot = boardSnapshot;
        PlayerIndex = playerIndex;
    }
}

// Manages undo and redo history using two stacks.
// Past moves can be undone; undone moves can be redone.
public class MoveHistory
{
    private readonly Stack<RecordedMove> _past = new();
    private readonly Stack<RecordedMove> _future = new();

    public int PastCount => _past.Count;

    public bool CanUndo(bool hvc)
    {
        if (hvc)
            return _past.Count >= 2;
        return _past.Count >= 1;
    }

    public bool CanRedo(bool hvc)
    {
        if (hvc)
            return _future.Count >= 2;
        return _future.Count >= 1;
    }

    // Save a move and clear the redo stack
    public void RecordMove(Move move, Board boardBefore, int playerIndex)
    {
        _past.Push(new RecordedMove(move, boardBefore, playerIndex));
        _future.Clear();
    }

    public RecordedMove? UndoSingle()
    {
        if (_past.Count == 0)
            return null;

        RecordedMove undone = _past.Pop();
        _future.Push(undone);
        return undone;
    }

    // Undo the last two moves (human then computer) for human vs computer mode
    public bool UndoRound(out RecordedMove? humanMove, out RecordedMove? computerMove)
    {
        humanMove = null;
        computerMove = null;

        if (_past.Count < 2)
            return false;

        computerMove = _past.Pop();
        humanMove = _past.Pop();
        _future.Push(computerMove);
        _future.Push(humanMove);
        return true;
    }

    public RecordedMove? RedoSingle()
    {
        if (_future.Count == 0)
            return null;

        RecordedMove redo = _future.Pop();
        _past.Push(redo);
        return redo;
    }

    // Redo the last two undone moves for human vs computer mode
    public bool RedoRound(out RecordedMove? humanMove, out RecordedMove? computerMove)
    {
        humanMove = null;
        computerMove = null;

        if (_future.Count < 2)
            return false;

        humanMove = _future.Pop();
        computerMove = _future.Pop();
        _past.Push(humanMove);
        _past.Push(computerMove);
        return true;
    }

    // Return all recorded moves in the order they were played
    public List<Move> GetHistory()
    {
        List<RecordedMove> ordered = new List<RecordedMove>(_past);
        ordered.Reverse();

        List<Move> moves = new List<Move>();
        foreach (RecordedMove recorded in ordered)
            moves.Add(recorded.Move);

        return moves;
    }

    // Rebuild the history stack from a list of saved moves
    public void RebuildFromMoves(IReadOnlyList<Move> moves, Board board)
    {
        _past.Clear();
        _future.Clear();

        foreach (Move move in moves)
        {
            Board snapshot = board.Clone();
            board.ApplyMove(move);
            int player = move.PlayerIndex;
            _past.Push(new RecordedMove(move, snapshot, player));
        }
    }
}
