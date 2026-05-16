using BoardGame.Moves;
using BoardGame.SaveLoad;

namespace BoardGame.Core;
public abstract class Game
{
    protected Board     Board     = null!;
    protected Player[]  Players;
    protected int       CurrentIdx;
    protected bool      GameOver;
    protected Player?   Winner;
    protected MoveHistory History = new();
    protected bool        _resumeFromSave;

    public abstract string GameName { get; }
    protected Player Current => Players[CurrentIdx];

    protected Game(Player[] players) { Players = players; }

    //  Main loop
    public void Play()
    {
        if (!_resumeFromSave)
            SetupBoard();
        _resumeFromSave = false;

        Console.WriteLine($"\n=== {GameName} ===");
        Console.WriteLine("Commands: M=Move  U=Undo  R=Redo  S=Save  H=Help\n");

        while (!GameOver)
        {
            Board.Display();
            Console.WriteLine($"\n{Current.Name}'s turn [{Current.Symbol}]");

            if (Current is Players.HumanPlayer)
            {
                Console.Write("Action: ");
                string cmd = (Console.ReadLine() ?? "m").Trim().ToLower();
                switch (cmd)
                {
                    case "u": Undo();     continue;
                    case "r": Redo();     continue;
                    case "s": SaveGame(); continue;
                    case "h": ShowHelp(); continue;
                }
            }

            Move? move = Current.GetMove(Board, this);
            if (move != null) DoMove(move);
        }

        Board.Display();
        AnnounceResult();
    }

    //  Move execution 
    protected virtual void DoMove(Move move)
    {
        History.Execute(new MoveCommand(move, Board));

        if      (CheckWin())   { Winner = Current; GameOver = true; }
        else if (!HasMoves())  { GameOver = true; }
        else CurrentIdx = (CurrentIdx + 1) % Players.Length;
    }

    //  Undo / Redo 
    public void Undo()
    {
        if (!History.CanUndo()) { Console.WriteLine("Nothing to undo."); return; }
        History.Undo();
        CurrentIdx = (CurrentIdx - 1 + Players.Length) % Players.Length;
        GameOver = false; Winner = null;
        Console.WriteLine("Move undone.");
    }

    public void Redo()
    {
        if (!History.CanRedo()) { Console.WriteLine("Nothing to redo."); return; }
        History.Redo();
        CurrentIdx = (CurrentIdx + 1) % Players.Length;
        Console.WriteLine("Move redone.");
    }

    //  Save 
    public void SaveGame()
    {
        Console.Write("Filename (no extension): ");
        string name = Console.ReadLine()?.Trim() ?? "save";
        Console.Write("Format [T]xt / [J]son: ");
        string fmt = Console.ReadLine()?.Trim().ToLower() ?? "t";

        ISaveStrategy strategy = fmt.StartsWith('j')
            ? new JsonSaveStrategy()
            : new TextSaveStrategy();

        strategy.Save(CreateGameState(), name);
        Console.WriteLine("Game saved.");
    }

    //  Help 
    private void ShowHelp()
    {
        Console.WriteLine("\n--- Help ---");
        Console.WriteLine("  M  Make a move");
        Console.WriteLine("  U  Undo last move");
        Console.WriteLine("  R  Redo undone move");
        Console.WriteLine("  S  Save game");
        Console.WriteLine("  H  Show this help");
        ShowGameHelp();
        Console.WriteLine();
    }

    protected virtual void ShowGameHelp() { }   // OCP: each game can add tips

    //  Result 
    protected virtual void AnnounceResult()
        => Console.WriteLine(Winner != null
            ? $"\n*** {Winner.Name} wins! ***"
            : "\n*** Draw! ***");

    //  Abstract hooks 
    protected abstract void       SetupBoard();
    protected abstract bool       CheckWin();
    protected abstract bool       HasMoves();
    public    abstract List<Move> GetValidMoves();
    public    abstract Move       PromptHumanMove(Player p);
    public    abstract bool       CheckWinOnBoard(Board b);
    public    abstract GameState  CreateGameState();
    public    abstract void       LoadFromState(GameState s);
}
