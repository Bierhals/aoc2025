using day2;

// Erwartet: 1 Argument = Pfad zur Eingabedatei
if (args.Length < 1)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
	Environment.Exit(2);
}

var filePath = args[0];

try
{
    ProductIdRange[] idRanges = ReadProductIdRangesFromInputFile(filePath);
    long SumInvalidIds = 0;
    long SumInvalidIds2 = 0;

    foreach (var idRange in idRanges)
    {
        var invalidIds = FindInvalidIds(idRange);
        var invalidIds2 = FindInvalidIds2(idRange);
        Console.WriteLine($"- {idRange.FirstId}-{idRange.LastId} has {invalidIds2.Length} invalid IDs; {string.Join(", ", invalidIds2)}");
        SumInvalidIds += invalidIds.Sum(x => x);
        SumInvalidIds2 += invalidIds2.Sum(x => x);
    }
    Console.WriteLine($"Sum of all invalid Product IDs: {SumInvalidIds}");
    Console.WriteLine($"Sum of all invalid Product IDs (repeated): {SumInvalidIds2}");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
	Environment.Exit(4);
}

static ProductIdRange[] ReadProductIdRangesFromInputFile(string path)
{
    var input = File.ReadAllText(path);
    Console.WriteLine($"Datei gelesen: {path}");

    if (string.IsNullOrWhiteSpace(input))
    {
        throw new Exception("Eingabedatei ist leer.");
    }

    var idGroups = input
        .Split(',')
        .Select(group =>
        {
            var ids = group.Split('-');
            if (ids.Length != 2 ||
                !long.TryParse(ids[0], out long firstId) ||
                !long.TryParse(ids[1], out long lastId))
            {
                throw new Exception("Ungültiges Format für ProductIdRange. Erwartet: <FirstId>-<LastId>");
            }
            return new ProductIdRange(firstId, lastId);
        }).ToArray();

    return idGroups;
}

static long[] FindInvalidIds(ProductIdRange range)
{
    var invalidIds = new List<long>();
    for (long id = range.FirstId; id <= range.LastId; id++)
    {
        if (!IsValidProductId(id))
        {
            invalidIds.Add(id);
        }
    }
    return invalidIds.ToArray();
}

static bool IsValidProductId(long productId)
{
    var idText = productId.ToString();
    if (idText.Length % 2 != 0)
    {
        return true;
    }
    return idText.Substring(0, idText.Length / 2) != idText.Substring(idText.Length / 2);
}

static long[] FindInvalidIds2(ProductIdRange range)
{
    var invalidIds = new List<long>();
    for (long id = range.FirstId; id <= range.LastId; id++)
    {
        if (!IsValidProductId2(id))
        {
            invalidIds.Add(id);
        }
    }
    return invalidIds.ToArray();
}

static bool IsValidProductId2(long productId)
{
    var idText = productId.ToString();
    for (int len = 1; len <= idText.Length / 2; len++)
    {
        if (idText.Length % len != 0)
            continue;
        
        if (idText == string.Concat(Enumerable.Repeat(idText.Substring(0, len), idText.Length / len)))
            return false;
    }

    return true;
}