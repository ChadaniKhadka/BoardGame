using BoardGame.Factory;

Console.WriteLine("+-----------------------------------+");
Console.WriteLine("|   Two-Player Board Game Framework |");
Console.WriteLine("+-----------------------------------+");

bool running = true;
while (running)
{
    Console.WriteLine("\n  1. New Game");
    Console.WriteLine("  2. Load Saved Game");
    Console.WriteLine("  3. Exit");
    Console.Write("  Choice: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": NewGame();  break;
        case "2": LoadGame(); break;
        case "3": running = false; break;
        default:  Console.WriteLine("  Please enter 1, 2 or 3."); break;
    }
}

Console.WriteLine("\n  Goodbye!\n");

//  New game 
static void NewGame()
{
    Console.WriteLine("\n  --- Select Game ---");
    Console.WriteLine("  1. Tic-Tac-Toe          (3x3, 3 in a row)");
    Console.WriteLine("  2. Numerical Tic-Tac-Toe (3x3, line sum 15)");
    Console.WriteLine("  3. Notakto              (3x3, last line loses)");
    Console.WriteLine("  4. Gomoku               (15x15, 5 in a row)");
    Console.WriteLine("  5. Connect Four         (6x7, 4 in a row)");
    Console.Write("  Choice (1-5): ");

    if (!int.TryParse(Console.ReadLine()?.Trim(), out int gc) || gc < 1 || gc > 5)
    { Console.WriteLine("  Invalid choice."); return; }

    Console.WriteLine("\n  --- Mode ---");
    Console.WriteLine("  1. Human vs Human");
    Console.WriteLine("  2. Human vs Computer");
    Console.Write("  Choice: ");
    string mode = Console.ReadLine()?.Trim() ?? "1";

    var players = GameFactory.BuildPlayers(mode);

    try   { GameFactory.Create(gc, players).Play(); }
    catch (Exception ex) { Console.WriteLine($"  Error: {ex.Message}"); }
}

//  Load saved game ─
static void LoadGame()
{
    Console.Write("\n  Filename (.txt or .json): ");
    string fn = Console.ReadLine()?.Trim() ?? "";
    try   { GameFactory.Load(fn).Play(); }
    catch (Exception ex) { Console.WriteLine($"  Could not load: {ex.Message}"); }
}
