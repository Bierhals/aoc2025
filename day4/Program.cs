// Erwartet: 1 Argument = Pfad zur Eingabedatei
if (args.Length < 1)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
	Environment.Exit(2);
}

var filePath = args[0];

try
{
    var paperGrid = ReadPaperGridFromInputFile(filePath);
    var accessableRolls = 0;

    while (true)
    {
        var removedRolls = RemoveAccessableRolls(paperGrid);
        accessableRolls += removedRolls;
        
        if (removedRolls == 0)
        {
            break;
        }
        Console.WriteLine($"{removedRolls} rolls have been removed.");
    }

    PrintGrid(paperGrid);
    Console.WriteLine($"{accessableRolls} rolls of paper are accessable.");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
	Environment.Exit(4);
}

static char[,] ReadPaperGridFromInputFile(string path)
{
    var lines = File.ReadAllLines(path);
    Console.WriteLine($"Datei gelesen: {path}");
    Console.WriteLine($"Zeilenanzahl: {lines.Length}");

    int rowCount = lines.Length;
    int colCount = lines[0].Length;
    var grid = new char[rowCount, colCount];

    for (int r = 0; r < rowCount; r++)
    {
        for (int c = 0; c < colCount; c++)
        {
            grid[r, c] = lines[r][c];
        }
    }

    return grid;
}

static bool RollIsAccessable(char[,] grid, int row, int col)
{
    if (grid[row, col] != '@')
    {
        return false;
    }

    var fromRow = row == 0 ? 0 : row - 1;
    var toRow = row == grid.GetLength(0) - 1 ? grid.GetLength(0) - 1 : row + 1;
    var fromCol = col == 0 ? 0 : col - 1;
    var toCol = col == grid.GetLength(1) - 1 ? grid.GetLength(1) - 1 : col + 1;
    var neighborCount = 0;

    for (int r = fromRow; r <= toRow; r++)
    {
        for (int c = fromCol; c <= toCol; c++)
        {
            if (c == col && r == row)
            {
                continue;
            }
            if (grid[r, c] == '@' || grid[r, c] == 'x')
            {
                neighborCount++;
            }
        }
    }
    return neighborCount < 4;
}

static void PrintGrid(char[,] grid)
{
    for (int r = 0; r < grid.GetLength(0); r++)
    {
        for (int c = 0; c < grid.GetLength(1); c++)
        {
            Console.Write(grid[r, c]);
        }
        Console.WriteLine();
    }
}

static int RemoveAccessableRolls(char[,] paperGrid)
{
    var accessableRolls = 0;

    // Mark accessable rolls
    for (int r = 0; r < paperGrid.GetLength(0); r++)
    {
        for (int c = 0; c < paperGrid.GetLength(1); c++)
        {
            if (RollIsAccessable(paperGrid, r, c))
            {
                paperGrid[r, c] = 'x';
            }
        }
    }

    // Count and remove accessable rolls
    for (int r = 0; r < paperGrid.GetLength(0); r++)
    {
        for (int c = 0; c < paperGrid.GetLength(1); c++)
        {
            if (paperGrid[r, c] == 'x')
            {
                paperGrid[r, c] = '.';
                accessableRolls++;
            }
        }
    }

    return accessableRolls;
}