using BoardGame.Core;
using BoardGame.Games;
using BoardGame.Players;
using BoardGame.SaveLoad;

namespace BoardGame.Factory;

// SRP: only knows how to build and restore Game instances
// OCP: add a new game by adding one line to the switch — nothing else changes
// DIP: returns Game (abstraction), not concrete game types
public static class GameFactory
{
    // ── Create a fresh game ───────────────────────────────────────────────
    public static Game Create(int choice, Player[] players) => choice switch
    {
        1 => new TicTacToeGame(players),
        2 => new NumericalTicTacToeGame(players),
        3 => new NotaktoGame(players),
        4 => new GomokuGame(players),
        5 => new ConnectFourGame(players),
        _ => throw new ArgumentException("Unknown game selection.")
    };

    // ── Load a saved game ─────────────────────────────────────────────────
    public static Game Load(string filename)
    {
        // DIP: choose strategy by file extension, not hardcoded type
        ISaveStrategy strategy = filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? new JsonSaveStrategy()
            : new TextSaveStrategy();

        GameState s = strategy.Load(filename);

        Player[] players = Enumerable.Range(0, s.PlayerNames.Length)
            .Select(i => s.PlayerTypes[i] == "Computer"
                ? (Player)new ComputerPlayer(s.PlayerNames[i], s.PlayerSymbols[i], i)
                :          new HumanPlayer  (s.PlayerNames[i], s.PlayerSymbols[i], i))
            .ToArray();

        Game game = s.GameType switch
        {
            "tictactoe"           => new TicTacToeGame(players),
            "numericaltictactoe"  => new NumericalTicTacToeGame(players),
            "notakto"             => new NotaktoGame(players),
            "gomoku"              => new GomokuGame(players),
            "connectfour"         => new ConnectFourGame(players),
            _ => throw new InvalidDataException($"Unknown game type: {s.GameType}")
        };

        game.LoadFromState(s);
        return game;
    }

    // ── Build players from user input ─────────────────────────────────────
    public static Player[] BuildPlayers(string mode)
    {
        string n1 = Prompt("Player 1", "Alice");
        Player p1 = new HumanPlayer(n1, 'X', 0);
        Player p2 = mode == "2"
            ? new ComputerPlayer("Computer", 'O', 1)
            : new HumanPlayer(Prompt("Player 2 name", "Bob"), 'O', 1);
        return [p1, p2];
    }

    // ── Helper ────────────────────────────────────────────────────────────
    private static string Prompt(string label, string fallback)
    {
        Console.Write($"  {label} (default '{fallback}'): ");
        string v = Console.ReadLine()?.Trim() ?? "";
        return string.IsNullOrWhiteSpace(v) ? fallback : v;
    }
}
