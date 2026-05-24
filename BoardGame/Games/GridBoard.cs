using BoardGame.Core;

namespace BoardGame.Games;

//  GridMove 
public class GridMove : Move
{
    public int  Row   { get; set; }
    public int  Col   { get; set; }
    public char Value { get; set; }  // symbol or digit placed

    public override string Serialize() => $"{PlayerIndex},{Row},{Col},{Value}";

    public static GridMove Deserialize(string s)
    {
        var p = s.Split(',');
        return new GridMove
        {
            PlayerIndex = int.Parse(p[0]),
            Row         = int.Parse(p[1]),
            Col         = int.Parse(p[2]),
            Value       = p[3][0]
        };
    }
}

//  GridBoard 
public class GridBoard : Board
{
    public  int    Rows { get; }
    public  int    Cols { get; }
    public  char[,] Cells { get; private set; }
    private char   _empty;

    public GridBoard(int rows, int cols, char empty = '.')
    {
        Rows = rows; Cols = cols; _empty = empty;
        Cells = new char[rows, cols];
        Reset();
    }

    public void Reset()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                Cells[r, c] = _empty;
    }

    public bool IsEmpty(int r, int c) => Cells[r, c] == _empty;

    public override bool ApplyMove(Move move)
    {
        var m = (GridMove)move;
        if (!IsEmpty(m.Row, m.Col)) return false;
        Cells[m.Row, m.Col] = m.Value;
        return true;
    }

    public override bool UndoMove(Move move)
    {
        var m = (GridMove)move;
        Cells[m.Row, m.Col] = _empty;
        return true;
    }

    public override bool IsFull()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (IsEmpty(r, c)) return false;
        return true;
    }

    // public override void Display()
    // {
    //     // Column header
    //     Console.Write("   ");
    //     for (int c = 0; c < Cols; c++) Console.Write($" {c + 1}");
    //     Console.WriteLine();

    //     for (int r = 0; r < Rows; r++)
    //     {
    //         Console.Write($" {r + 1} ");
    //         for (int c = 0; c < Cols; c++)
    //             Console.Write($" {Cells[r, c]}");
    //         Console.WriteLine();
    //     }
    // }
    public override void Display()
{
    Console.WriteLine();

    string border = "+" + new string('-', Cols * 4 - 1) + "+";
    bool isConnectFour = Rows == 6 && Cols == 7;

    Console.WriteLine(border);

    for (int r = 0; r < Rows; r++)
    {
        Console.Write("|");

        for (int c = 0; c < Cols; c++)
        {
            char cell = Cells[r, c];

            if (cell == '.' || cell == ' ' || cell == '\0')
                Console.Write("   |");
            else
                Console.Write($" {cell} |");
        }

        Console.WriteLine($"   row {Rows - r}");

        if (!isConnectFour && r < Rows - 1)
        {
            Console.WriteLine(new string('-', Cols * 4 + 1));
        }
    }

    Console.WriteLine(border);

    Console.Write(" ");
    for (int c = 1; c <= Cols; c++)
    {
        Console.Write($" {c}  ");
    }

    Console.WriteLine(" columns");
    Console.WriteLine();
}

    public override Board Clone()
    {
        var copy = new GridBoard(Rows, Cols, _empty);
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                copy.Cells[r, c] = Cells[r, c];
        return copy;
    }

    public override string Serialize()
    {
        var sb = new System.Text.StringBuilder();
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                sb.Append(Cells[r, c]);
        return sb.ToString();
    }

    public override void Deserialize(string data)
    {
        int idx = 0;
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                Cells[r, c] = idx < data.Length ? data[idx++] : _empty;
    }
}
