namespace BoardGame.UI;

public class HelpMenu
{
    // commands that work in every game
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

    // how to play this game
    public static void ShowGameHelp(string gameHelp)
    {
        if (!string.IsNullOrWhiteSpace(gameHelp))
        {
            Console.WriteLine("\n--- How to play ---");
            Console.WriteLine(gameHelp);
            Console.WriteLine("-------------------");
        }
    }

    // show commands and game help together
    public static void Show(string gameHelp)
    {
        ShowCommands();
        ShowGameHelp(gameHelp);
    }
}
