using day6;

// Erwartet: 1 Argument = Pfad zur Eingabedatei
if (args.Length < 1)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
	Environment.Exit(2);
}

var filePath = args[0];

try
{
    var problems = ReadProblemsFromInputFile(filePath);
    var grandTotal = 0L;

    foreach (var problem in problems)
    {
        var result = problem.Solve();
        Console.WriteLine($"- {problem} = {result}");
        grandTotal += result;
    }
    Console.WriteLine($"Grand total: {grandTotal}");

    problems = ReadCephalopodProblemsFromInputFile(filePath);
    grandTotal = 0L;

    foreach (var problem in problems)
    {
        var result = problem.Solve();
        Console.WriteLine($"- {problem} = {result}");
        grandTotal += result;
    }
    Console.WriteLine($"Grand total (Cephalopod): {grandTotal}");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
	Environment.Exit(4);
}

static List<Problem> ReadProblemsFromInputFile(string path)
{
    var lines = File.ReadAllLines(path)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(x =>
        {
            var line = x.Trim();
            while (line.Contains("  "))
            {
              line = line.Replace("  ", " ");
            }

            return line.Split(' ');
        })
        .ToArray();
    Console.WriteLine($"Datei gelesen: {path}");
    Console.WriteLine($"Zeilenanzahl: {lines.Length}");
    var problems = new List<Problem>();

    for (int i = 0; i < lines[0].Length; i++)
    {
        problems.Add(
            new Problem(
                lines[lines.Length-1][i][0], 
                lines.Take(lines.Length-1).Select(x => long.Parse(x[i])).ToArray()));
    }

    return problems;
}


static List<Problem> ReadCephalopodProblemsFromInputFile(string path)
{
    var lines = File.ReadAllLines(path);
    Console.WriteLine($"Datei gelesen: {path}");
    Console.WriteLine($"Zeilenanzahl: {lines.Length}");

    var parts = new List<string[]>();
    var lastIndex = 0;
    var rowParts = new List<string>();
    
    // read Operators row
    Console.WriteLine("Lese Operatoren-Zeile");
    for (int i = 1; i < lines[lines.Length-1].Length; i++)
    {
        if (lines[lines.Length-1][i] != ' ')
        {
            rowParts.Add(lines[lines.Length-1][lastIndex..i]);
            lastIndex = i;
        }
    }
    rowParts.Add(lines[lines.Length-1][lastIndex..]);
    parts.Add(rowParts.ToArray());

    // read Values rows
    Console.WriteLine("Lese Werte-Zeilen");
    for (int r = 0; r < lines.Length - 1; r++)
    {
        lastIndex = 0;
        rowParts = new List<string>();
        for (int i = 0; i < parts[0].Length; i++)
        {
            rowParts.Add(lines[r][lastIndex..(lastIndex + parts[0][i].Length)]);
            lastIndex += parts[0][i].Length;
        }
        parts.Add(rowParts.ToArray());
    }
    
    // convert to Problems
    Console.WriteLine("Erzeuge Probleme");
    var problems = new List<Problem>();

    for (int i = parts[0].Length-1; i >= 0; i--)
    {
        var mathOperator = parts[0][i][0];
        var chars = parts.Skip(1)
            .Select(x => x[i].ToArray()).ToArray();
        
        var maxLength = chars.Max(x => x.Length);
        var values = new List<long>();
        for (int pos = 0; pos < maxLength; pos++)
        {
            var value = chars
                .Select(c => c.Length-1-pos < c.Length ? c[c.Length-1-pos] : ' ')
                .ToArray();
            
            if (string.IsNullOrWhiteSpace(new string(value)))
            {
                continue;
            }
            values.Add(long.Parse(new string(value).Trim()));
        }
        problems.Add(
            new Problem(
                mathOperator, 
                values.ToArray()));
    }

    return problems;
}