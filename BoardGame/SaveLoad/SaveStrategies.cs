using System.Text.Json;
using BoardGame.Core;

namespace BoardGame.SaveLoad;

public interface ISaveStrategy
{
    void      Save(GameState state, string filename);
    GameState Load(string filename);
    string    GetFileExtension();
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

    private string Serialise(GameState s)
    {
        var lines = new List<string>
        {
            $"GameType={s.GameType}",
            $"BoardData={s.BoardData}",
            $"CurrentIdx={s.CurrentPlayerIndex}",
            $"Count={s.PlayerNames.Length}",
        };

        for (int i = 0; i < s.PlayerNames.Length; i++)
        {
            lines.Add($"N{i}={s.PlayerNames[i]}");
            lines.Add($"S{i}={s.PlayerSymbols[i]}");
            lines.Add($"T{i}={s.PlayerTypes[i]}");
        }

        lines.Add($"Moves={s.MoveHistory.Count}");
        for (int i = 0; i < s.MoveHistory.Count; i++)
            lines.Add($"M{i}={s.MoveHistory[i]}");

        foreach (var kv in s.Extra)
            lines.Add($"X_{kv.Key}={kv.Value}");

        return string.Join(Environment.NewLine, lines);
    }

    private GameState Deserialise(string data)
    {
        var d = data.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Where(l => l.Contains('='))
                    .ToDictionary(
                        l => l[..l.IndexOf('=')],
                        l => l[(l.IndexOf('=') + 1)..]);

        int n = int.Parse(d.GetValueOrDefault("Count", "2"));

        var gs = new GameState
        {
            GameType           = d.GetValueOrDefault("GameType", ""),
            BoardData          = d.GetValueOrDefault("BoardData", ""),
            CurrentPlayerIndex = int.Parse(d.GetValueOrDefault("CurrentIdx", "0")),
            PlayerNames        = Enumerable.Range(0, n).Select(i => d[$"N{i}"]).ToArray(),
            PlayerSymbols      = Enumerable.Range(0, n).Select(i => d[$"S{i}"][0]).ToArray(),
            PlayerTypes        = Enumerable.Range(0, n).Select(i => d[$"T{i}"]).ToArray(),
        };

        int mc = int.Parse(d.GetValueOrDefault("Moves", "0"));
        for (int i = 0; i < mc; i++) gs.MoveHistory.Add(d[$"M{i}"]);

        foreach (var kv in d.Where(k => k.Key.StartsWith("X_")))
            gs.Extra[kv.Key[2..]] = kv.Value;

        return gs;
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
