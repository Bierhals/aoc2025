namespace day7;

class Program
{
    public static void Main(string[] args)
    {
        // Erwartet: 1 Argument = Pfad zur Eingabedatei
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
            Environment.Exit(2);
        }

        var filePath = args[0];

        try
        {
            var tachyonManifold = ReadTachyonManifoldFromInputFile(filePath);

            PrintTachyonManifold(tachyonManifold);
            var (newTachyonManifold, tachyonBeamSplitted) = SimulateTachyonBeam(tachyonManifold);
            var timelines = SimulateTimelines(tachyonManifold);
            PrintTachyonManifold(newTachyonManifold);
            Console.WriteLine($"Tachyon beam splitted: {tachyonBeamSplitted} times.");
            Console.WriteLine($"Number of timelines created: {timelines}.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
            Environment.Exit(4);
        }

    }

    static Dictionary<(int x, int y), long> PositionTimelines = new Dictionary<(int x, int y), long>();

    static char[,] ReadTachyonManifoldFromInputFile(string path)
    {
        var lines = File.ReadAllLines(path)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
        Console.WriteLine($"Datei gelesen: {path}");
        Console.WriteLine($"Zeilenanzahl: {lines.Length}");
        if (lines.Length == 0)
        {
            throw new Exception("Eingabedatei ist leer.");
        }

        var rowCount = lines.Length;
        var colCount = lines[0].Length;
        var manifold = new char[rowCount, colCount];

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                manifold[r, c] = lines[r][c];
            }
        }

        return manifold;
    }

    static void PrintTachyonManifold(char[,] manifold)
    {
        var rowCount = manifold.GetLength(0);
        var colCount = manifold.GetLength(1);

        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                Console.Write(manifold[r, c]);
            }
            Console.WriteLine();
        }
    }

    static (char[,], int) SimulateTachyonBeam(char[,] manifold)
    {
        var newtachyonManifold = manifold.Clone() as char[,];
        var splitted = 0;

        for (int r = 0; r < newtachyonManifold.GetUpperBound(0); r++)
        {
            for (int c = 0; c < newtachyonManifold.GetUpperBound(1); c++)
            {
                if (newtachyonManifold[r, c] == 'S')
                {
                    splitted += MoveTachyonBeam(newtachyonManifold, r + 1, c);
                }
            }
        }

        return (newtachyonManifold, splitted);
    }

    static int MoveTachyonBeam(char[,] manifold, int row, int col)
    {
        if (row > manifold.GetUpperBound(0))
        {
            return 0;
        }
        if (col < 0 || col > manifold.GetUpperBound(1))
        {
            return 0;
        }

        if (manifold[row, col] == '.')
        {
            manifold[row, col] = '|';
            return MoveTachyonBeam(manifold, row + 1, col);
        }

        if (manifold[row, col] == '^')
        {
            var splitted = 1;
            splitted += MoveTachyonBeam(manifold, row, col - 1);
            splitted += MoveTachyonBeam(manifold, row, col + 1);
            return splitted;
        }

        return 0;
    }

    static long SimulateTimelines(char[,] manifold)
    {
        var newtachyonManifold = manifold.Clone() as char[,];
        var timelines = 0L;

        for (int r = 0; r < newtachyonManifold.GetUpperBound(0); r++)
        {
            for (int c = 0; c < newtachyonManifold.GetUpperBound(1); c++)
            {
                if (newtachyonManifold[r, c] == 'S')
                {
                    Console.WriteLine($"Simuliere Timeline ab Position ({r},{c})");
                    timelines += MoveTachyonBeamInTimeline(newtachyonManifold, r + 1, c);
                }
            }
        }

        return timelines;
    }

    static long MoveTachyonBeamInTimeline(char[,] manifold, int row, int col)
    {
        if (row > manifold.GetUpperBound(0))
        {
            return 0;
        }
        if (col < 0 || col > manifold.GetUpperBound(1))
        {
            return 0;
        }
        if (PositionTimelines.ContainsKey((row, col)))
        {
            return PositionTimelines[(row, col)];
        }

        if (manifold[row, col] == '.')
        {
            var timelines = MoveTachyonBeamInTimeline(manifold, row + 1, col);
            return timelines == 0 ? 1 : timelines;
        }

        if (manifold[row, col] == '^')
        {
            var timelines = 0L;
            timelines += MoveTachyonBeamInTimeline(manifold, row, col - 1);
            timelines += MoveTachyonBeamInTimeline(manifold, row, col + 1);
            PositionTimelines.Add((row, col), timelines);
            return timelines;
        }

        return 1;
    }
}