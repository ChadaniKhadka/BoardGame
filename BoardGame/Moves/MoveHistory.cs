using BoardGame.Core;

namespace BoardGame.Moves;
public interface ICommand { void Execute(); void Undo(); }


public class MoveCommand : ICommand
{
    public  Move  Move   { get; }
    private Board _board;

    public MoveCommand(Move move, Board board) { Move = move; _board = board; }

    public void Execute() => _board.ApplyMove(Move);
    public void Undo()    => _board.UndoMove(Move);
}
public class MoveHistory
{
    private readonly Stack<ICommand> _undo = new();
    private readonly Stack<ICommand> _redo = new();

    public bool CanUndo() => _undo.Count > 0;
    public bool CanRedo() => _redo.Count > 0;
    public int  Count     => _undo.Count;

    public void Execute(ICommand cmd)
    {
        cmd.Execute();
        _undo.Push(cmd);
        _redo.Clear();          // new move invalidates redo stack
    }

    public void Undo()
    {
        if (!CanUndo()) return;
        var c = _undo.Pop();
        c.Undo();
        _redo.Push(c);
    }

    public void Redo()
    {
        if (!CanRedo()) return;
        var c = _redo.Pop();
        c.Execute();
        _undo.Push(c);
    }

    // Returns moves in chronological order (oldest first)
    public List<Move> GetHistory() =>
        _undo.Select(c => ((MoveCommand)c).Move).Reverse().ToList();
}
