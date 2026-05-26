using BoardGame.Moves;
using BoardGame.Players;
using BoardGame.SaveLoad;
using BoardGame.UI;

namespace BoardGame.Core;

// Base class for all board games. Runs the main play loop and handles undo/redo/save.
public abstract class Game
{
    protected Board Board { get; set; } = null!;
    protected Player[] Players { get; set; }
    protected int CurrentIdx { get; set; }
    protected bool GameOver { get; set; }
    protected Player? Winner { get; set; }
    protected MoveHistory History { get; } = new();
    protected bool _resumeFromSave;

    public abstract string GameName { get; }
    protected Player Current => Players[CurrentIdx];

    protected Game(Player[] players)
    {
        Players = players;
    }

    private bool IsHvCMode()
    {
        foreach (Player player in Players)
        {
            if (player is ComputerPlayer)
                return true;
        }
        return false;
    }

    private int GetHumanPlayerIndex()
    {
        foreach (Player player in Players)
        {
            if (player is HumanPlayer)
                return player.Index;
        }
        return 0;
    }

    // Run the main game loop until the game ends or the player exits
    public void Play()
    {
        if (!_resumeFromSave)
            SetupBoard();
        _resumeFromSave = false;

        Console.WriteLine($"\n========== {GameName} ==========");

        bool exitRequested = false;
        while (!GameOver && !exitRequested)
        {
            Board.Display();

            if (Current is HumanPlayer)
                exitRequested = ProcessHumanTurn();
            else
                ProcessComputerTurn();
        }

        if (GameOver)
        {
            Board.Display();
            AnnounceResult();
        }
    }

    // Read and process one human player's turn
    private bool ProcessHumanTurn()
    {
        Console.WriteLine($"\nPlayer {CurrentIdx + 1}'s turn");
        ShowTurnInfo(Current);
        Console.Write($"P{CurrentIdx + 1}> ");

        string input = Console.ReadLine()?.Trim() ?? "";

        if (TryHandleHumanCommand(input, out bool exitRequested))
            return exitRequested;

        Move? move = PromptHumanMove(Current, input);
        if (move is not null)
            DoMove(move);

        return false;
    }

    // Handle undo, redo, save, help, and exit commands
    private bool TryHandleHumanCommand(string input, out bool exitRequested)
    {
        exitRequested = false;
        string command = input.ToLower();

        switch (command)
        {
            case "u":
                Undo();
                return true;
            case "r":
                Redo();
                return true;
            case "s":
                SaveGame();
                return true;
            case "h":
                ShowHelp();
                return true;
            case "e":
                exitRequested = ExitGame();
                return true;
            default:
                return false;
        }
    }

    // Let the computer player choose and apply a move
    private void ProcessComputerTurn()
    {
        Console.WriteLine($"\nPlayer {CurrentIdx + 1}'s turn");
        Move? move = Current.GetMove(Board, this);
        if (move is null)
        {
            Console.WriteLine("No moves available.");
            GameOver = true;
            return;
        }
        DoMove(move);
    }

    protected virtual void DoMove(Move move)
    {
        Board snapshot = Board.Clone();
        int mover = CurrentIdx;

        Board.ApplyMove(move);
        History.RecordMove(move, snapshot, mover);

        FinalizeAfterMove(mover);
    }

    // Update game state after a move: check win, draw, or switch player
    private void FinalizeAfterMove(int playerWhoMoved)
    {
        if (CheckWin())
        {
            Winner = Players[playerWhoMoved];
            GameOver = true;
        }
        else if (!HasMoves())
        {
            GameOver = true;
        }
        else
        {
            CurrentIdx = (CurrentIdx + 1) % Players.Length;
        }
    }

    protected virtual void OnBoardRestored() { }

    // Restore the board from a saved snapshot
    protected void RestoreBoard(Board snapshot)
    {
        Board.Deserialize(snapshot.Serialize());
        OnBoardRestored();
    }

    public void Undo()
    {
        if (IsHvCMode())
            UndoHvC();
        else
            UndoHvH();
    }

    // Undo the last single move (human vs human mode)
    private void UndoHvH()
    {
        if (!History.CanUndo(hvc: false))
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        RecordedMove? undone = History.UndoSingle();
        if (undone is null)
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        RestoreBoard(undone.BoardSnapshot);
        CurrentIdx = undone.PlayerIndex;
        GameOver = false;
        Winner = null;
        Console.WriteLine("Move undone.");
    }

    // Undo the last human and computer move pair (human vs computer mode)
    private void UndoHvC()
    {
        if (!History.CanUndo(hvc: true))
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        if (!History.UndoRound(out RecordedMove? humanMove, out _))
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        RestoreBoard(humanMove!.BoardSnapshot);
        CurrentIdx = humanMove.PlayerIndex;
        GameOver = false;
        Winner = null;
        Console.WriteLine("Move undone.");
    }

    public void Redo()
    {
        if (IsHvCMode())
            RedoHvC();
        else
            RedoHvH();
    }

    // Redo the last undone move (human vs human mode)
    private void RedoHvH()
    {
        if (!History.CanRedo(hvc: false))
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        RecordedMove? redo = History.RedoSingle();
        if (redo is null)
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        GameOver = false;
        Winner = null;

        Board.ApplyMove(redo.Move);
        FinalizeAfterMove(redo.PlayerIndex);
        Console.WriteLine("Move redone.");
    }

    // Redo the last undone human and computer move pair
    private void RedoHvC()
    {
        if (!History.CanRedo(hvc: true))
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        if (!History.RedoRound(out RecordedMove? humanMove, out RecordedMove? computerMove))
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        GameOver = false;
        Winner = null;

        Board.ApplyMove(humanMove!.Move);
        Board.ApplyMove(computerMove!.Move);

        if (CheckWin())
        {
            Winner = Players[computerMove.PlayerIndex];
            GameOver = true;
        }
        else if (!HasMoves())
        {
            GameOver = true;
        }
        else
        {
            CurrentIdx = GetHumanPlayerIndex();
        }

        Console.WriteLine("Move redone.");
    }

    public void SaveGame()
    {
        Console.Write("Filename (no extension): ");
        string filename = Console.ReadLine()?.Trim() ?? "save";
        Console.Write("Format [T]xt / [J]son: ");
        string format = Console.ReadLine()?.Trim() ?? "t";

        ISaveStrategy strategy = SaveStrategyFactory.ForFormat(format);
        strategy.Save(CreateGameState(), filename);
        Console.WriteLine("Game saved.");
    }

    private void ShowHelp()
    {
        HelpMenu.Show(GetGameHelp());
    }

    private bool ExitGame()
    {
        Console.Write("\n  Exit to main menu? (y/n): ");
        string confirm = Console.ReadLine()?.Trim().ToLower() ?? "n";
        if (confirm != "y")
            return false;

        if (History.PastCount > 0)
        {
            Console.Write("  You have unsaved progress. Save before exiting? (y/n): ");
            string save = Console.ReadLine()?.Trim().ToLower() ?? "n";
            if (save == "y")
                SaveGame();
        }

        Console.WriteLine("  Returning to main menu...");
        return true;
    }

    public virtual void ShowTurnInfo(Player currentPlayer)
    {
        Console.WriteLine("Enter Your Move [row <space> col (e.g. 1 2)] or Commands:  U=Undo  R=Redo  S=Save  H=Help  E=Exit");
    }

    protected abstract string GetGameHelp();

    protected virtual void AnnounceResult()
    {
        if (Winner is not null)
            Console.WriteLine($"\n*** {Winner.Name} wins! ***");
        else
            Console.WriteLine("\n*** Draw! ***");
    }

    protected abstract void SetupBoard();
    protected abstract bool CheckWin();
    protected abstract bool HasMoves();
    public abstract List<Move> GetValidMoves();
    public abstract Move? PromptHumanMove(Player p, string input);
    public abstract bool CheckWinOnBoard(Board b);
    public abstract GameState CreateGameState();
    public abstract void LoadFromState(GameState s);
}
