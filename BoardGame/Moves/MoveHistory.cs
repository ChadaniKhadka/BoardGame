using BoardGame.Core;

namespace BoardGame.Moves;

// one move plus the board before it was played
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

// keeps track of moves so player can undo and redo
// undo goes back redo brings moves forward again
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

    // save a move and forget any redo history
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

    // take back human and computer moves in one go
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

    // put back human and computer moves in one go
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

    // list all moves in the order they were played
    public List<Move> GetHistory()
    {
        List<RecordedMove> ordered = new List<RecordedMove>(_past);
        ordered.Reverse();

        List<Move> moves = new List<Move>();
        foreach (RecordedMove recorded in ordered)
            moves.Add(recorded.Move);

        return moves;
    }

    // rebuild move history after loading a saved game
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
