using BoardGame.Core;
using BoardGame.Players;

namespace BoardGame.Games;

// shared save load and reading row col moves
public abstract class BaseGame : Game
{
    protected GridBoard Grid => (GridBoard)Board;

    protected BaseGame(Player[] players) : base(players) { }

    public override GameState CreateGameState()
    {
        string[] playerNames = new string[Players.Length];
        char[] playerSymbols = new char[Players.Length];
        string[] playerTypes = new string[Players.Length];

        for (int i = 0; i < Players.Length; i++)
        {
            playerNames[i] = Players[i].Name;
            playerSymbols[i] = Players[i].Symbol;
            if (Players[i] is ComputerPlayer)
                playerTypes[i] = "Computer";
            else
                playerTypes[i] = "Human";
        }

        List<string> moveHistory = new List<string>();
        List<Move> moves = History.GetHistory();
        foreach (Move move in moves)
            moveHistory.Add(move.Serialize());

        string gameType = GameName.ToLower();
        gameType = gameType.Replace(" ", "");
        gameType = gameType.Replace("-", "");

        GameState state = new GameState();
        state.GameType = gameType;
        state.BoardData = Board.Serialize();
        state.CurrentPlayerIndex = CurrentIdx;
        state.PlayerNames = playerNames;
        state.PlayerSymbols = playerSymbols;
        state.PlayerTypes = playerTypes;
        state.MoveHistory = moveHistory;
        return state;
    }

    public override void LoadFromState(GameState s)
    {
        _resumeFromSave = true;
        SetupBoard();
        Board emptyBoard = Board.Clone();
        Board.Deserialize(s.BoardData);
        CurrentIdx = s.CurrentPlayerIndex;

        List<Move> moves = new List<Move>();
        foreach (string moveText in s.MoveHistory)
            moves.Add(GridMove.Deserialize(moveText));

        History.RebuildFromMoves(moves, emptyBoard);
    }

    public override Move? PromptHumanMove(Player p, string input)
    {
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            Console.WriteLine("  Invalid or occupied cell, try again.");
            return null;
        }

        if (!int.TryParse(parts[0], out int row))
        {
            Console.WriteLine("  Invalid or occupied cell, try again.");
            return null;
        }

        if (!int.TryParse(parts[1], out int col))
        {
            Console.WriteLine("  Invalid or occupied cell, try again.");
            return null;
        }

        if (row < 1 || row > Grid.Rows || col < 1 || col > Grid.Cols)
        {
            Console.WriteLine("  Invalid or occupied cell, try again.");
            return null;
        }

        if (!Grid.IsEmpty(row - 1, col - 1))
        {
            Console.WriteLine("  Invalid or occupied cell, try again.");
            return null;
        }

        return new GridMove
        {
            PlayerIndex = p.Index,
            Row = row - 1,
            Col = col - 1,
            Value = p.Symbol
        };
    }
}
