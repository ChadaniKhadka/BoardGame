using System.Text.Json;
using BoardGame.Core;

namespace BoardGame.SaveLoad;

// INTERFACE
public interface ISaveStrategy
{
    void Save(GameState state, string filename);
    GameState Load(string filename);
    string GetFileExtension();
}


// TEXT SAVE STRATEGY
public class TextSaveStrategy : ISaveStrategy
{
    public string GetFileExtension() => ".txt";

    public void Save(GameState state, string filename)
    {
        string path = AddExtension(filename);
        File.WriteAllText(path, Serialize(state));
    }

    public GameState Load(string filename)
    {
        string path = AddExtension(filename);
        return Deserialize(File.ReadAllText(path));
    }

    private string Serialize(GameState s)
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

    private GameState Deserialize(string data)
    {
        var dict = data.Split(Environment.NewLine)
            .Where(l => l.Contains('='))
            .ToDictionary(
                l => l[..l.IndexOf('=')],
                l => l[(l.IndexOf('=') + 1)..]
            );

        int n = int.Parse(dict.GetValueOrDefault("Count", "2"));

        var state = new GameState
        {
            GameType = dict["GameType"],
            BoardData = dict["BoardData"],
            CurrentPlayerIndex = int.Parse(dict["CurrentIdx"]),

            PlayerNames = Enumerable.Range(0, n).Select(i => dict[$"N{i}"]).ToArray(),
            PlayerSymbols = Enumerable.Range(0, n).Select(i => dict[$"S{i}"][0]).ToArray(),
            PlayerTypes = Enumerable.Range(0, n).Select(i => dict[$"T{i}"]).ToArray(),
        };

        int moveCount = int.Parse(dict.GetValueOrDefault("Moves", "0"));

        for (int i = 0; i < moveCount; i++)
            state.MoveHistory.Add(dict[$"M{i}"]);

        foreach (var kv in dict.Where(k => k.Key.StartsWith("X_")))
            state.Extra[kv.Key[2..]] = kv.Value;

        return state;
    }

    private string AddExtension(string filename)
    {
        return filename.EndsWith(GetFileExtension(), StringComparison.OrdinalIgnoreCase)
            ? filename
            : filename + GetFileExtension();
    }
}


// JSON SAVE STRATEGY
public class JsonSaveStrategy : ISaveStrategy
{
    private static readonly JsonSerializerOptions Opts =
        new() { WriteIndented = true };

    public string GetFileExtension() => ".json";

    public void Save(GameState state, string filename)
    {
        string path = AddExtension(filename);
        File.WriteAllText(path, JsonSerializer.Serialize(state, Opts));
    }

    public GameState Load(string filename)
    {
        string path = AddExtension(filename);

        return JsonSerializer.Deserialize<GameState>(File.ReadAllText(path))
               ?? throw new InvalidDataException("Bad save file.");
    }

    private string AddExtension(string filename)
    {
        return filename.EndsWith(GetFileExtension(), StringComparison.OrdinalIgnoreCase)
            ? filename
            : filename + GetFileExtension();
    }
}


// FACTORY (FIXED)
public static class SaveStrategyFactory
{
    // Use filename (.json / .txt)
    public static ISaveStrategy ForFilename(string filename)
    {
        return filename.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? new JsonSaveStrategy()
            : new TextSaveStrategy();
    }

    public static ISaveStrategy ForFormat(string format)
    {
        return format.Trim().ToLower() switch
        {
            "j" or "json" => new JsonSaveStrategy(),
            "t" or "txt" => new TextSaveStrategy(),
            _ => new TextSaveStrategy()
        };
    }
}