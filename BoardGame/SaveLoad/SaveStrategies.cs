using System.Text.Json;
using BoardGame.Core;

namespace BoardGame.SaveLoad;

public interface ISaveStrategy
{
    void      Save(GameState state, string name);
    GameState Load(string name);
}

//  Plain text 
public class TextSaveStrategy : ISaveStrategy
{
    public void Save(GameState s, string name)
    {
        if (!name.EndsWith(".txt")) name += ".txt";
        using var w = new StreamWriter(name);

        w.WriteLine($"GameType={s.GameType}");
        w.WriteLine($"BoardData={s.BoardData}");
        w.WriteLine($"CurrentIdx={s.CurrentPlayerIndex}");
        w.WriteLine($"Count={s.PlayerNames.Length}");

        for (int i = 0; i < s.PlayerNames.Length; i++)
        {
            w.WriteLine($"N{i}={s.PlayerNames[i]}");
            w.WriteLine($"S{i}={s.PlayerSymbols[i]}");
            w.WriteLine($"T{i}={s.PlayerTypes[i]}");
        }

        w.WriteLine($"Moves={s.MoveHistory.Count}");
        for (int i = 0; i < s.MoveHistory.Count; i++)
            w.WriteLine($"M{i}={s.MoveHistory[i]}");

        foreach (var kv in s.Extra)
            w.WriteLine($"X_{kv.Key}={kv.Value}");
    }

    public GameState Load(string name)
    {
        if (!name.EndsWith(".txt")) name += ".txt";

        var d = File.ReadAllLines(name)
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
}

//  JSON 
public class JsonSaveStrategy : ISaveStrategy
{
    private static readonly JsonSerializerOptions Opts = new() { WriteIndented = true };

    public void Save(GameState s, string name)
    {
        if (!name.EndsWith(".json")) name += ".json";
        File.WriteAllText(name, JsonSerializer.Serialize(s, Opts));
    }

    public GameState Load(string name)
    {
        if (!name.EndsWith(".json")) name += ".json";
        return JsonSerializer.Deserialize<GameState>(File.ReadAllText(name))
               ?? throw new InvalidDataException("Bad save file.");
    }
}
