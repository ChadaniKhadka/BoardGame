using BoardGame.Core;
using BoardGame.Players;

namespace BoardGame.Games;
public abstract class BaseGame : Game
{
    protected GridBoard Grid => (GridBoard)Board;

    protected BaseGame(Player[] players) : base(players) { }

    //  Common save/load 
    public override GameState CreateGameState() => new()
    {
        GameType           = GameName.ToLower().Replace(" ", "").Replace("-", ""),
        BoardData          = Board.Serialize(),
        CurrentPlayerIndex = CurrentIdx,
        PlayerNames        = Players.Select(p => p.Name).ToArray(),
        PlayerSymbols      = Players.Select(p => p.Symbol).ToArray(),
        PlayerTypes        = Players.Select(p => p is ComputerPlayer ? "Computer" : "Human").ToArray(),
        MoveHistory        = History.GetHistory().Select(m => m.Serialize()).ToList()
    };

    public override void LoadFromState(GameState s)
    {
        _resumeFromSave = true;
        SetupBoard();
        Board.Deserialize(s.BoardData);
        CurrentIdx = s.CurrentPlayerIndex;

        var moves = s.MoveHistory.Select(GridMove.Deserialize).Cast<Move>().ToList();
        History.Restore(moves, Board);
    }

    //  Common prompt ─
    // Subclasses override only when they need different input (e.g. Numerical)
    public override Move PromptHumanMove(Player p)
    {
        while (true)
        {
            Console.Write($"  Enter row col (e.g. 1 2): ");
            string? line = Console.ReadLine()?.Trim();
            var parts = line?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts?.Length == 2
                && int.TryParse(parts[0], out int row)
                && int.TryParse(parts[1], out int col)
                && row >= 1 && row <= Grid.Rows
                && col >= 1 && col <= Grid.Cols
                && Grid.IsEmpty(row - 1, col - 1))
            {
                return new GridMove
                {
                    PlayerIndex = p.Index,
                    Row         = row - 1,
                    Col         = col - 1,
                    Value       = p.Symbol
                };
            }
            Console.WriteLine("  Invalid or occupied cell — try again.");
        }
    }
}
