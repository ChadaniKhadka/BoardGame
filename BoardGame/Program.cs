using BoardGame.Core;
using BoardGame.Factory;

namespace BoardGame;

// Entry point for the board game framework console application.
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("+-------------------------------------+");
        Console.WriteLine("|   Two-Player Board Game Framework   |");
        Console.WriteLine("+-------------------------------------+");

        bool running = true;
        while (running)
        {
            ShowMainMenu();
            running = HandleMainMenuChoice();
        }

        Console.WriteLine("\n  Goodbye!\n");
    }

    private static void ShowMainMenu()
    {
        Console.WriteLine("+------------------------------+");
        Console.WriteLine("|          MAIN MENU           |");
        Console.WriteLine("+------------------------------+");
        Console.WriteLine("|  1. New Game                 |");
        Console.WriteLine("|  2. Load Saved Game          |");
        Console.WriteLine("|  3. Exit                     |");
        Console.WriteLine("+------------------------------+");
        Console.Write("Choice: ");
    }

    private static bool HandleMainMenuChoice()
    {
        string? choice = Console.ReadLine()?.Trim();

        switch (choice)
        {
            case "1":
                NewGame();
                return true;
            case "2":
                LoadGame();
                return true;
            case "3":
                bool staying = !ExitGame();
                return staying;
            default:
                Console.WriteLine("  Please enter 1, 2 or 3.");
                return true;
        }
    }

    private static bool ExitGame()
    {
        Console.Write("  Are you sure you want to exit? (y/n): ");
        string answer = Console.ReadLine()?.Trim().ToLower() ?? "n";
        return answer == "y";
    }

    private static void NewGame()
    {
        int? gameChoice = PromptGameChoice();
        if (gameChoice is null)
            return;

        string mode = PromptGameMode();
        Player[] players = GameFactory.BuildPlayers(mode);

        try
        {
            Game game = GameFactory.Create(gameChoice.Value, players);
            game.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }
    }

    private static int? PromptGameChoice()
    {
        Console.WriteLine("+-----------------------------------------------------------+");
        Console.WriteLine("|                       SELECT GAME                         |");
        Console.WriteLine("+-----------------------------------------------------------+");
        Console.WriteLine("|  1. Tic-Tac-Toe             (3x3, 3 in a row)             |");
        Console.WriteLine("|  2. Numerical Tic-Tac-Toe   (3x3, line sum 15)            |");
        Console.WriteLine("|  3. Notakto                 (3x3, last line loses)        |");
        Console.WriteLine("|  4. Gomoku                  (15x15, 5 in a row)           |");
        Console.WriteLine("|  5. Connect Four            (6x7, 4 in a row)             |");
        Console.WriteLine("+-----------------------------------------------------------+");
        Console.Write("Choice: ");

        if (!int.TryParse(Console.ReadLine()?.Trim(), out int gameChoice))
        {
            Console.WriteLine("  Invalid choice.");
            return null;
        }

        if (gameChoice < 1 || gameChoice > 5)
        {
            Console.WriteLine("  Invalid choice.");
            return null;
        }

        return gameChoice;
    }

    private static string PromptGameMode()
    {
        Console.WriteLine("+------------------------------+");
        Console.WriteLine("|            SELECT MODE       |");
        Console.WriteLine("+------------------------------+");
        Console.WriteLine("|  1. Human vs Human           |");
        Console.WriteLine("|  2. Human vs Computer        |");
        Console.WriteLine("+------------------------------+");
        Console.Write("Choice: ");
        return Console.ReadLine()?.Trim() ?? "1";
    }

    private static void LoadGame()
    {
        Console.Write("\n  Filename (.txt or .json): ");
        string filename = Console.ReadLine()?.Trim() ?? "";

        try
        {
            Game game = GameFactory.Load(filename);
            game.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Could not load: {ex.Message}");
        }
    }
}
