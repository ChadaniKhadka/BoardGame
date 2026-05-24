using BoardGame.Core;

namespace BoardGame.Moves;

public class RecordedMove
{
    public Move   Move           { get; }
    public Board  BoardSnapshot  { get; }
    public int    PlayerIndex    { get; }

    public RecordedMove(Move move, Board boardSnapshot, int playerIndex)
    {
        Move          = move;
        BoardSnapshot = boardSnapshot;
        PlayerIndex   = playerIndex;
    }
}

public class MoveHistory
{
    private readonly Stack<RecordedMove> _past   = new();
    private readonly Stack<RecordedMove> _future = new();

    public int PastCount   => _past.Count;
    public int FutureCount => _future.Count;

    public bool CanUndo(bool hvc) => hvc ? _past.Count >= 2 : _past.Count >= 1;
    public bool CanRedo(bool hvc) => hvc ? _future.Count >= 2 : _future.Count >= 1;

    public void RecordMove(Move move, Board boardBefore, int playerIndex)
    {
        _past.Push(new RecordedMove(move, boardBefore, playerIndex));
        _future.Clear();
    }

    public RecordedMove? UndoSingle()
    {
        if (_past.Count == 0) return null;
        var undone = _past.Pop();
        _future.Push(undone);
        return undone;
    }

    public bool UndoRound(out RecordedMove? humanMove, out RecordedMove? computerMove)
    {
        humanMove = computerMove = null;
        if (_past.Count < 2) return false;

        computerMove = _past.Pop();
        humanMove    = _past.Pop();
        _future.Push(computerMove);
        _future.Push(humanMove);
        return true;
    }

    public RecordedMove? RedoSingle()
    {
        if (_future.Count == 0) return null;
        var redo = _future.Pop();
        _past.Push(redo);
        return redo;
    }

    public bool RedoRound(out RecordedMove? humanMove, out RecordedMove? computerMove)
    {
        humanMove = computerMove = null;
        if (_future.Count < 2) return false;

        humanMove    = _future.Pop();
        computerMove = _future.Pop();
        _past.Push(humanMove);
        _past.Push(computerMove);
        return true;
    }

    public List<Move> GetHistory() =>
        _past.Reverse().Select(r => r.Move).ToList();

    public void RebuildFromMoves(IReadOnlyList<Move> moves, Board board)
    {
        _past.Clear();
        _future.Clear();

        foreach (var move in moves)
        {
            var snapshot = board.Clone();
            board.ApplyMove(move);
            int player = move.PlayerIndex;
            _past.Push(new RecordedMove(move, snapshot, player));
        }
    }
}
