using BoardGame.Moves;
using BoardGame.Players;
using BoardGame.SaveLoad;

namespace BoardGame.Core;

public abstract class Game
{
    protected Board Board = null!;
    protected Player[] Players;
    protected int CurrentIdx;
    protected bool GameOver;
    protected Player? Winner;
    protected MoveHistory History = new();
    protected bool _resumeFromSave;

    public abstract string GameName { get; }
    protected Player Current => Players[CurrentIdx];

    protected Game(Player[] players) { Players = players; }

    private bool IsHvCMode => Players.Any(p => p is ComputerPlayer);

    private int HumanPlayerIndex =>
        Players.First(p => p is HumanPlayer).Index;

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
            {
                Console.WriteLine($"\nPlayer {CurrentIdx + 1}'s turn");

                ShowTurnInfo(Current);

                Console.Write($"P{CurrentIdx + 1}> ");

                string input = Console.ReadLine()?.Trim() ?? "";

                switch (input.ToLower())
                {
                    case "u":
                        Undo();
                        continue;

                    case "r":
                        Redo();
                        continue;

                    case "s":
                        SaveGame();
                        continue;

                    case "h":
                        ShowHelp();
                        continue;

                    case "e":
                        exitRequested = ExitGame();
                        continue;
                }

                Move? move = PromptHumanMove(Current, input);
                if (move != null)
                    DoMove(move);
            }
            else
            {
                Console.WriteLine($"\nPlayer {CurrentIdx + 1}'s turn");
                Move? move = Current.GetMove(Board, this);
                if (move != null)
                    DoMove(move);
            }
        }

        if (GameOver)
        {
            Board.Display();
            AnnounceResult();
        }
    }

    protected virtual void DoMove(Move move)
    {
        Board snapshot = Board.Clone();
        int mover = CurrentIdx;

        Board.ApplyMove(move);
        History.RecordMove(move, snapshot, mover);

        FinalizeAfterMove(mover);
    }

    private void ApplyMoveWithoutRecording(Move move)
    {
        Board.ApplyMove(move);
    }

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

    protected void RestoreBoard(Board snapshot)
    {
        Board.Deserialize(snapshot.Serialize());
        OnBoardRestored();
    }

    public void Undo()
    {
        if (IsHvCMode)
            UndoHvC();
        else
            UndoHvH();
    }

    private void UndoHvH()
    {
        if (!History.CanUndo(hvc: false))
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        var undone = History.UndoSingle();
        if (undone == null)
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

    private void UndoHvC()
    {
        if (!History.CanUndo(hvc: true))
        {
            Console.WriteLine("Nothing to undo.");
            return;
        }

        if (!History.UndoRound(out var humanMove, out _))
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
        if (IsHvCMode)
            RedoHvC();
        else
            RedoHvH();
    }

    private void RedoHvH()
    {
        if (!History.CanRedo(hvc: false))
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        var redo = History.RedoSingle();
        if (redo == null)
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        GameOver = false;
        Winner = null;

        ApplyMoveWithoutRecording(redo.Move);
        FinalizeAfterMove(redo.PlayerIndex);
        Console.WriteLine("Move redone.");
    }

    private void RedoHvC()
    {
        if (!History.CanRedo(hvc: true))
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        if (!History.RedoRound(out var humanMove, out var computerMove))
        {
            Console.WriteLine("Nothing to redo.");
            return;
        }

        GameOver = false;
        Winner = null;

        ApplyMoveWithoutRecording(humanMove!.Move);
        ApplyMoveWithoutRecording(computerMove!.Move);

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
            CurrentIdx = HumanPlayerIndex;
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
        Console.WriteLine("\n--- Help ---");
        Console.WriteLine("  U  Undo last move");
        Console.WriteLine("  R  Redo undone move");
        Console.WriteLine("  S  Save game");
        Console.WriteLine("  H  Show this help");
        Console.WriteLine("  E  Exit to main menu");
        ShowGameHelp();
        Console.WriteLine();
    }

    private bool ExitGame()
    {
        Console.Write("\n  Exit to main menu? (y/n): ");
        string confirm = Console.ReadLine()?.Trim().ToLower() ?? "n";
        if (confirm != "y") return false;

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

    protected virtual void ShowGameHelp() { }

    protected virtual void AnnounceResult()
        => Console.WriteLine(Winner != null
            ? $"\n*** {Winner.Name} wins! ***"
            : "\n*** Draw! ***");

    protected abstract void SetupBoard();
    protected abstract bool CheckWin();
    protected abstract bool HasMoves();
    public abstract List<Move> GetValidMoves();
    public abstract Move? PromptHumanMove(Player p, string input);
    public abstract bool CheckWinOnBoard(Board b);
    public abstract GameState CreateGameState();
    public abstract void LoadFromState(GameState s);
}