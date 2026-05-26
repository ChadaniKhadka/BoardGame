using System.Text.Json;
using BoardGame.Core;

namespace BoardGame.SaveLoad;

public interface ISaveStrategy
{
    void Save(GameState state, string filename);
    GameState Load(string filename);
    string GetFileExtension();
}

public class TextSaveStrategy : ISaveStrategy
{
    public string GetFileExtension() => ".txt";

    public void Save(GameState state, string filename)
    {
        string path = WithExtension(filename);
        File.WriteAllText(path, Serialise(state));
    }

    public GameState Load(string filename)
    {
        string path = WithExtension(filename);
        return Deserialise(File.ReadAllText(path));
    }

    private string Serialise(GameState state)
    {
        List<string> lines = BuildHeaderLines(state);
        AppendPlayerLines(lines, state);
        AppendMoveLines(lines, state);
        AppendExtraLines(lines, state);
        return string.Join(Environment.NewLine, lines);
    }

    private static List<string> BuildHeaderLines(GameState state) =>
    [
        $"GameType={state.GameType}",
        $"BoardData={state.BoardData}",
        $"CurrentIdx={state.CurrentPlayerIndex}",
        $"Count={state.PlayerNames.Length}",
    ];

    private static void AppendPlayerLines(List<string> lines, GameState state)
    {
        for (int i = 0; i < state.PlayerNames.Length; i++)
        {
            lines.Add($"N{i}={state.PlayerNames[i]}");
            lines.Add($"S{i}={state.PlayerSymbols[i]}");
            lines.Add($"T{i}={state.PlayerTypes[i]}");
        }
    }

    private static void AppendMoveLines(List<string> lines, GameState state)
    {
        lines.Add($"Moves={state.MoveHistory.Count}");
        for (int i = 0; i < state.MoveHistory.Count; i++)
            lines.Add($"M{i}={state.MoveHistory[i]}");
    }

    private static void AppendExtraLines(List<string> lines, GameState state)
    {
        foreach (KeyValuePair<string, string> entry in state.Extra)
            lines.Add($"X_{entry.Key}={entry.Value}");
    }

    private GameState Deserialise(string data)
    {
        Dictionary<string, string> fields = ParseFields(data);
        int playerCount = int.Parse(fields.GetValueOrDefault("Count", "2"));

        GameState gameState = new GameState
        {
            GameType = fields.GetValueOrDefault("GameType", ""),
            BoardData = fields.GetValueOrDefault("BoardData", ""),
            CurrentPlayerIndex = int.Parse(fields.GetValueOrDefault("CurrentIdx", "0")),
            PlayerNames = Enumerable.Range(0, playerCount).Select(i => fields[$"N{i}"]).ToArray(),
            PlayerSymbols = Enumerable.Range(0, playerCount).Select(i => fields[$"S{i}"][0]).ToArray(),
            PlayerTypes = Enumerable.Range(0, playerCount).Select(i => fields[$"T{i}"]).ToArray(),
        };

        LoadMoveHistory(gameState, fields);
        LoadExtraFields(gameState, fields);
        return gameState;
    }

    private static Dictionary<string, string> ParseFields(string data) =>
        data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.Contains('='))
            .ToDictionary(
                line => line[..line.IndexOf('=')],
                line => line[(line.IndexOf('=') + 1)..]);

    private static void LoadMoveHistory(GameState gameState, Dictionary<string, string> fields)
    {
        int moveCount = int.Parse(fields.GetValueOrDefault("Moves", "0"));
        for (int i = 0; i < moveCount; i++)
            gameState.MoveHistory.Add(fields[$"M{i}"]);
    }

    private static void LoadExtraFields(GameState gameState, Dictionary<string, string> fields)
    {
        foreach (KeyValuePair<string, string> entry in fields.Where(k => k.Key.StartsWith("X_")))
            gameState.Extra[entry.Key[2..]] = entry.Value;
    }

    private string WithExtension(string filename) =>
        filename.EndsWith(GetFileExtension(), StringComparison.OrdinalIgnoreCase)
            ? filename
            : filename + GetFileExtension();
}

public class JsonSaveStrategy : ISaveStrategy
{
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public string GetFileExtension() => ".json";

    public void Save(GameState state, string filename)
    {
        string path = WithExtension(filename);
        File.WriteAllText(path, SerialiseToJson(state));
    }

    public GameState Load(string filename)
    {
        string path = WithExtension(filename);
        return DeserialiseFromJson(File.ReadAllText(path));
    }

    private string SerialiseToJson(GameState state) =>
        JsonSerializer.Serialize(state, Opts);

    private GameState DeserialiseFromJson(string json) =>
        JsonSerializer.Deserialize<GameState>(json)
        ?? throw new InvalidDataException("Bad save file.");

    private string WithExtension(string filename) =>
        filename.EndsWith(GetFileExtension(), StringComparison.OrdinalIgnoreCase)
            ? filename
            : filename + GetFileExtension();
}

public static class SaveStrategyFactory
{
    public static ISaveStrategy ForFilename(string filename) =>
        filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? new JsonSaveStrategy()
            : new TextSaveStrategy();

    public static ISaveStrategy ForFormat(string format) =>
        format.ToLower() == "json"
            ? (ISaveStrategy)new JsonSaveStrategy()
            : new TextSaveStrategy();
}
