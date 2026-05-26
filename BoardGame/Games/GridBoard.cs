using System.Text;
using BoardGame.Core;

namespace BoardGame.Games;

public class GridMove : Move
{
    public int Row { get; set; }
    public int Col { get; set; }
    public char Value { get; set; }

    public override string Serialize() => $"{PlayerIndex},{Row},{Col},{Value}";

    public static GridMove Deserialize(string s)
    {
        string[] parts = s.Split(',');
        return new GridMove
        {
            PlayerIndex = int.Parse(parts[0]),
            Row = int.Parse(parts[1]),
            Col = int.Parse(parts[2]),
            Value = parts[3][0]
        };
    }
}

public class GridBoard : Board
{
    private const int ConnectFourRows = 6;
    private const int ConnectFourCols = 7;
    private const int CellDisplayWidth = 4;

    public int Rows { get; }
    public int Cols { get; }
    public char[,] Cells { get; private set; }
    private readonly char _empty;

    public GridBoard(int rows, int cols, char empty = '.')
    {
        Rows = rows;
        Cols = cols;
        _empty = empty;
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
        GridMove gridMove = (GridMove)move;
        if (!IsEmpty(gridMove.Row, gridMove.Col)) return false;
        Cells[gridMove.Row, gridMove.Col] = gridMove.Value;
        return true;
    }

    public override bool UndoMove(Move move)
    {
        GridMove gridMove = (GridMove)move;
        Cells[gridMove.Row, gridMove.Col] = _empty;
        return true;
    }

    public override bool IsFull()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                if (IsEmpty(r, c)) return false;
        return true;
    }

    public override void Display()
    {
        Console.WriteLine();
        string border = BuildBorder();
        bool isConnectFour = Rows == ConnectFourRows && Cols == ConnectFourCols;

        Console.WriteLine(border);

        for (int r = 0; r < Rows; r++)
        {
            PrintRow(r);
            Console.WriteLine($"   row {Rows - r}");

            if (!isConnectFour && r < Rows - 1)
                Console.WriteLine(new string('-', Cols * CellDisplayWidth + 1));
        }

        Console.WriteLine(border);
        PrintColumnLabels();
        Console.WriteLine();
    }

    private string BuildBorder() => "+" + new string('-', Cols * CellDisplayWidth - 1) + "+";

    private void PrintRow(int row)
    {
        Console.Write("|");
        for (int c = 0; c < Cols; c++)
        {
            char cell = Cells[row, c];
            if (cell == '.' || cell == ' ' || cell == '\0')
                Console.Write("   |");
            else
                Console.Write($" {cell} |");
        }
    }

    private void PrintColumnLabels()
    {
        Console.Write(" ");
        for (int c = 1; c <= Cols; c++)
            Console.Write($" {c}  ");
        Console.WriteLine(" columns");
    }

    public override Board Clone()
    {
        GridBoard copy = new GridBoard(Rows, Cols, _empty);
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                copy.Cells[r, c] = Cells[r, c];
        return copy;
    }

    public override string Serialize()
    {
        StringBuilder sb = new StringBuilder();
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
