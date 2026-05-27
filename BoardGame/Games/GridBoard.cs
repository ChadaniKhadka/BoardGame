using System.Text;
using BoardGame.Core;

namespace BoardGame.Games;

// a move on a row and column
public class GridMove : Move
{
    public int Row { get; set; }
    public int Col { get; set; }
    public char Value { get; set; }

    public override string Serialize()
    {
        return PlayerIndex + "," + Row + "," + Col + "," + Value;
    }

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

// grid of rows and columns with one symbol per cell
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
        {
            for (int c = 0; c < Cols; c++)
                Cells[r, c] = _empty;
        }
    }

    public bool IsEmpty(int r, int c)
    {
        return Cells[r, c] == _empty;
    }

    public override bool ApplyMove(Move move)
    {
        GridMove gridMove = (GridMove)move;
        if (!IsEmpty(gridMove.Row, gridMove.Col))
            return false;

        Cells[gridMove.Row, gridMove.Col] = gridMove.Value;
        return true;
    }

    public override bool IsFull()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (IsEmpty(r, c))
                    return false;
            }
        }
        return true;
    }

    public override void Display()
    {
        Console.WriteLine();

        string border = "+";
        for (int i = 0; i < Cols * CellDisplayWidth - 1; i++)
            border += "-";
        border += "+";

        bool isConnectFour = Rows == ConnectFourRows && Cols == ConnectFourCols;

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
                    Console.Write(" " + cell + " |");
            }

            int rowLabel = isConnectFour ? Rows - r : r + 1;
            Console.WriteLine("   row " + rowLabel);

            if (!isConnectFour && r < Rows - 1)
            {
                string separator = new string('-', Cols * CellDisplayWidth + 1);
                Console.WriteLine(separator);
            }
        }

        Console.WriteLine(border);

        Console.Write(" ");
        for (int c = 1; c <= Cols; c++)
            Console.Write(" " + c + "  ");
        Console.WriteLine(" columns");
        Console.WriteLine();
    }

    public override Board Clone()
    {
        GridBoard copy = new GridBoard(Rows, Cols, _empty);
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
                copy.Cells[r, c] = Cells[r, c];
        }
        return copy;
    }

    public override string Serialize()
    {
        StringBuilder sb = new StringBuilder();
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
                sb.Append(Cells[r, c]);
        }
        return sb.ToString();
    }

    public override void Deserialize(string data)
    {
        int idx = 0;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (idx < data.Length)
                {
                    Cells[r, c] = data[idx];
                    idx++;
                }
                else
                {
                    Cells[r, c] = _empty;
                }
            }
        }
    }
}
