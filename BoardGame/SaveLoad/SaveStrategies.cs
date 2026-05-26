using System.Text.Json;
using BoardGame.Core;

namespace BoardGame.SaveLoad;

public interface ISaveStrategy
{
    void Save(GameState state, string filename);
    GameState Load(string filename);
    string GetFileExtension();
}

// Saves and loads game state as plain text key=value lines.
public class TextSaveStrategy : ISaveStrategy
{
    public string GetFileExtension()
    {
        return ".txt";
    }

    public void Save(GameState state, string filename)
    {
        string path = WithExtension(filename);
        File.WriteAllText(path, Serialise(state));
    }

    public GameState Load(string filename)
    {
        string path = WithExtension(filename);
        string data = File.ReadAllText(path);
        return Deserialise(data);
    }

    // Write each piece of game state as one line: key=value
    private string Serialise(GameState state)
    {
        List<string> lines = new List<string>();

        lines.Add("GameType=" + state.GameType);
        lines.Add("BoardData=" + state.BoardData);
        lines.Add("CurrentIdx=" + state.CurrentPlayerIndex);
        lines.Add("Count=" + state.PlayerNames.Length);

        for (int i = 0; i < state.PlayerNames.Length; i++)
        {
            lines.Add("N" + i + "=" + state.PlayerNames[i]);
            lines.Add("S" + i + "=" + state.PlayerSymbols[i]);
            lines.Add("T" + i + "=" + state.PlayerTypes[i]);
        }

        lines.Add("Moves=" + state.MoveHistory.Count);
        for (int i = 0; i < state.MoveHistory.Count; i++)
            lines.Add("M" + i + "=" + state.MoveHistory[i]);

        foreach (KeyValuePair<string, string> entry in state.Extra)
            lines.Add("X_" + entry.Key + "=" + entry.Value);

        return string.Join(Environment.NewLine, lines);
    }

    // Read key=value lines back into a GameState object
    private GameState Deserialise(string data)
    {
        Dictionary<string, string> fields = new Dictionary<string, string>();
        string[] lines = data.Split(
            new[] { Environment.NewLine },
            StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (!line.Contains('='))
                continue;

            int equalsIndex = line.IndexOf('=');
            string key = line.Substring(0, equalsIndex);
            string value = line.Substring(equalsIndex + 1);
            fields[key] = value;
        }

        int playerCount = 2;
        if (fields.ContainsKey("Count"))
            playerCount = int.Parse(fields["Count"]);

        GameState gameState = new GameState();
        gameState.GameType = fields.ContainsKey("GameType") ? fields["GameType"] : "";
        gameState.BoardData = fields.ContainsKey("BoardData") ? fields["BoardData"] : "";
        gameState.CurrentPlayerIndex = fields.ContainsKey("CurrentIdx")
            ? int.Parse(fields["CurrentIdx"])
            : 0;

        gameState.PlayerNames = new string[playerCount];
        gameState.PlayerSymbols = new char[playerCount];
        gameState.PlayerTypes = new string[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            gameState.PlayerNames[i] = fields["N" + i];
            gameState.PlayerSymbols[i] = fields["S" + i][0];
            gameState.PlayerTypes[i] = fields["T" + i];
        }

        int moveCount = 0;
        if (fields.ContainsKey("Moves"))
            moveCount = int.Parse(fields["Moves"]);

        for (int i = 0; i < moveCount; i++)
            gameState.MoveHistory.Add(fields["M" + i]);

        foreach (KeyValuePair<string, string> entry in fields)
        {
            if (entry.Key.StartsWith("X_"))
            {
                string extraKey = entry.Key.Substring(2);
                gameState.Extra[extraKey] = entry.Value;
            }
        }

        return gameState;
    }

    // Add .txt extension if the filename does not already have it
    private string WithExtension(string filename)
    {
        if (filename.EndsWith(GetFileExtension(), StringComparison.OrdinalIgnoreCase))
            return filename;
        return filename + GetFileExtension();
    }
}

// Saves and loads game state as formatted JSON.
public class JsonSaveStrategy : ISaveStrategy
{
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public string GetFileExtension()
    {
        return ".json";
    }

    public void Save(GameState state, string filename)
    {
        string path = WithExtension(filename);
        string json = SerialiseToJson(state);
        File.WriteAllText(path, json);
    }

    public GameState Load(string filename)
    {
        string path = WithExtension(filename);
        string json = File.ReadAllText(path);
        return DeserialiseFromJson(json);
    }

    private string SerialiseToJson(GameState state)
    {
        return JsonSerializer.Serialize(state, Opts);
    }

    private GameState DeserialiseFromJson(string json)
    {
        GameState? state = JsonSerializer.Deserialize<GameState>(json);
        if (state is null)
            throw new InvalidDataException("Bad save file.");
        return state;
    }

    private string WithExtension(string filename)
    {
        if (filename.EndsWith(GetFileExtension(), StringComparison.OrdinalIgnoreCase))
            return filename;
        return filename + GetFileExtension();
    }
}

// Picks the correct save strategy based on file extension or user choice.
public static class SaveStrategyFactory
{
    public static ISaveStrategy ForFilename(string filename)
    {
        if (filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return new JsonSaveStrategy();
        return new TextSaveStrategy();
    }

    public static ISaveStrategy ForFormat(string format)
    {
        if (format.ToLower() == "json")
            return new JsonSaveStrategy();
        return new TextSaveStrategy();
    }
}
