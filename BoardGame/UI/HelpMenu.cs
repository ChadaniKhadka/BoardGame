namespace BoardGame.UI;

public class HelpMenu
{
    // General commands shown during any game
    public static void ShowCommands()
    {
        Console.WriteLine("\n--- Commands ---");
        Console.WriteLine("  u  Undo last move");
        Console.WriteLine("  r  Redo last undone move");
        Console.WriteLine("  s  Save game to file");
        Console.WriteLine("  h  Show this help menu");
        Console.WriteLine("  e  Exit to main menu");
        Console.WriteLine("----------------");
    }

    // Game-specific help shown below commands
    public static void ShowGameHelp(string gameHelp)
    {
        if (!string.IsNullOrWhiteSpace(gameHelp))
        {
            Console.WriteLine("\n--- How to play ---");
            Console.WriteLine(gameHelp);
            Console.WriteLine("-------------------");
        }
    }

    // Combined: show both at once
    public static void Show(string gameHelp)
    {
        ShowCommands();
        ShowGameHelp(gameHelp);
    }
}
