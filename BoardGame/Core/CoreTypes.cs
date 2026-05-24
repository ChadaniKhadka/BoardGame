namespace BoardGame.Core;

// Move
public abstract class Move
{
    public int PlayerIndex { get; set; }
    public abstract string Serialize();
}

// Board 
public abstract class Board
{
    public abstract bool   ApplyMove(Move move);
    public abstract bool   UndoMove(Move move);
    public abstract bool   IsFull();
    public abstract void   Display();
    public abstract Board  Clone();
    public abstract string Serialize();
    public abstract void   Deserialize(string data);
}

// Player 
public abstract class Player
{
    public string Name   { get; }
    public char   Symbol { get; }
    public int    Index  { get; }

    protected Player(string name, char symbol, int index)
    { Name = name; Symbol = symbol; Index = index; }

    public abstract Move? GetMove(Board board, Game game);
}

// GameState (save / load) 
public class GameState
{
    public string       GameType           { get; set; } = "";
    public string       BoardData          { get; set; } = "";
    public int          CurrentPlayerIndex { get; set; }
    public string[]     PlayerNames        { get; set; } = [];
    public char[]       PlayerSymbols      { get; set; } = [];
    public string[]     PlayerTypes        { get; set; } = [];
    public List<string> MoveHistory        { get; set; } = [];
    public Dictionary<string, string> Extra { get; set; } = [];
}
