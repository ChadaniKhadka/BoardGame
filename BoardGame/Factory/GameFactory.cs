using BoardGame.Core;
using BoardGame.Games;
using BoardGame.Players;
using BoardGame.SaveLoad;

namespace BoardGame.Factory;

// Creates new games and loads saved games from file.
public static class GameFactory
{
    public static Game Create(int choice, Player[] players)
    {
        switch (choice)
        {
            case 1: return new TicTacToeGame(players);
            case 2: return new NumericalTicTacToeGame(players);
            case 3: return new NotaktoGame(players);
            case 4: return new GomokuGame(players);
            case 5: return new ConnectFourGame(players);
            default: throw new ArgumentException("Unknown game selection.");
        }
    }

    public static Game Load(string filename)
    {
        ISaveStrategy strategy = SaveStrategyFactory.ForFilename(filename);
        GameState state = strategy.Load(filename);

        Player[] players = new Player[state.PlayerNames.Length];
        for (int i = 0; i < state.PlayerNames.Length; i++)
        {
            if (state.PlayerTypes[i] == "Computer")
            {
                players[i] = new ComputerPlayer(
                    state.PlayerNames[i],
                    state.PlayerSymbols[i],
                    i);
            }
            else
            {
                players[i] = new HumanPlayer(
                    state.PlayerNames[i],
                    state.PlayerSymbols[i],
                    i);
            }
        }

        Game game;
        switch (state.GameType)
        {
            case "tictactoe":
                game = new TicTacToeGame(players);
                break;
            case "numericaltictactoe":
                game = new NumericalTicTacToeGame(players);
                break;
            case "notakto":
                game = new NotaktoGame(players);
                break;
            case "gomoku":
                game = new GomokuGame(players);
                break;
            case "connectfour":
                game = new ConnectFourGame(players);
                break;
            default:
                throw new InvalidDataException($"Unknown game type: {state.GameType}");
        }

        game.LoadFromState(state);
        return game;
    }

    public static Player[] BuildPlayers(string mode)
    {
        Player playerOne = new HumanPlayer("P1", 'X', 0);

        Player playerTwo;
        if (mode == "2")
            playerTwo = new ComputerPlayer("P2", 'O', 1);
        else
            playerTwo = new HumanPlayer("P2", 'O', 1);

        return [playerOne, playerTwo];
    }
}
