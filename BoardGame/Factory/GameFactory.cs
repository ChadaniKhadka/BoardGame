using BoardGame.Core;
using BoardGame.Games;
using BoardGame.Players;
using BoardGame.SaveLoad;

namespace BoardGame.Factory;

public static class GameFactory
{
    public static Game Create(int choice, Player[] players) => choice switch
    {
        1 => new TicTacToeGame(players),
        2 => new NumericalTicTacToeGame(players),
        3 => new NotaktoGame(players),
        4 => new GomokuGame(players),
        5 => new ConnectFourGame(players),
        _ => throw new ArgumentException("Unknown game selection.")
    };

    public static Game Load(string filename)
    {
        ISaveStrategy strategy = SaveStrategyFactory.ForFilename(filename);
        GameState state = strategy.Load(filename);

        Player[] players = Enumerable.Range(0, state.PlayerNames.Length)
            .Select(i => state.PlayerTypes[i] == "Computer"
                ? (Player)new ComputerPlayer(state.PlayerNames[i], state.PlayerSymbols[i], i)
                : new HumanPlayer(state.PlayerNames[i], state.PlayerSymbols[i], i))
            .ToArray();

        Game game = state.GameType switch
        {
            "tictactoe" => new TicTacToeGame(players),
            "numericaltictactoe" => new NumericalTicTacToeGame(players),
            "notakto" => new NotaktoGame(players),
            "gomoku" => new GomokuGame(players),
            "connectfour" => new ConnectFourGame(players),
            _ => throw new InvalidDataException($"Unknown game type: {state.GameType}")
        };

        game.LoadFromState(state);
        return game;
    }

    public static Player[] BuildPlayers(string mode)
    {
        Player playerOne = new HumanPlayer("P1", 'X', 0);

        Player playerTwo = mode == "2"
            ? new ComputerPlayer("P2", 'O', 1)
            : new HumanPlayer("P2", 'O', 1);

        return [playerOne, playerTwo];
    }
}
