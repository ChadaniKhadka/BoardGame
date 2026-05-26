using BoardGame.Core;
using BoardGame.Games;
using BoardGame.Players;
using BoardGame.SaveLoad;

namespace BoardGame.Factory;

public static class GameFactory
{
    // CREATE A NEW GAME
    public static Game Create(int gameChoice, Player[] players)
    {
        switch (gameChoice)
        {
            case 1:
                return new TicTacToeGame(players);

            case 2:
                return new NumericalTicTacToeGame(players);

            case 3:
                return new NotaktoGame(players);

            case 4:
                return new GomokuGame(players);

            case 5:
                return new ConnectFourGame(players);

            default:
                throw new ArgumentException("Invalid game choice.");
        }
    }

    // LOAD A SAVED GAME
    public static Game Load(string fileName)
    {
        // Choose save strategy based on file extension
        ISaveStrategy strategy =
            SaveStrategyFactory.ForFilename(fileName);

        // Load saved data
        GameState state = strategy.Load(fileName);

        // Create players from saved data
        Player[] players = new Player[state.PlayerNames.Length];

        for (int i = 0; i < state.PlayerNames.Length; i++)
        {
            // Create computer player
            if (state.PlayerTypes[i] == "Computer")
            {
                players[i] = new ComputerPlayer(
                    state.PlayerNames[i],
                    state.PlayerSymbols[i],
                    i
                );
            }
            // Create human player
            else
            {
                players[i] = new HumanPlayer(
                    state.PlayerNames[i],
                    state.PlayerSymbols[i],
                    i
                );
            }
        }

        Game game;

        // Create correct game type
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
                throw new InvalidDataException("Unknown game type.");
        }

        // Restore saved game data
        game.LoadFromState(state);

        return game;
    }


    // CREATE PLAYERS
    public static Player[] BuildPlayers(string mode)
    {
        // Player 1 is always human
        Player player1 = new HumanPlayer("P1", 'X', 0);

        Player player2;

        // If mode = 2, play against computer
        if (mode == "2")
        {
            player2 = new ComputerPlayer("P2", 'O', 1);
        }
        else
        {
            player2 = new HumanPlayer("P2", 'O', 1);
        }

        return new Player[] { player1, player2 };
    }
}