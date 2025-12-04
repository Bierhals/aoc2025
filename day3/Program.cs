// Erwartet: 1 Argument = Pfad zur Eingabedatei
if (args.Length < 1)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
	Environment.Exit(2);
}

var filePath = args[0];

try
{
    BatteriesBank[] banks = ReadBatteriesBanksFromInputFile(filePath);
    var sumMaxJoltage = 0;
    long sumMaxJoltage12 = 0;

    // Check all battery banks
    foreach (var bank in banks)
    {
        var maxJoltage = GetMaxJoltage(bank);
        var maxJoltage12 = GetMax12Joltage(bank);
        Console.WriteLine($"{string.Join("", bank.Batteries)}, max joltage: {maxJoltage}, max 12-joltage: {maxJoltage12}");
        sumMaxJoltage += maxJoltage;
        sumMaxJoltage12 += maxJoltage12;
    }
    Console.WriteLine($"Sum of all max joltage values: {sumMaxJoltage}");
    Console.WriteLine($"Sum of all max 12-joltage values: {sumMaxJoltage12}");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
	Environment.Exit(4);
}

static BatteriesBank[] ReadBatteriesBanksFromInputFile(string path)
{
    var lines = File.ReadAllLines(path);
    Console.WriteLine($"Datei gelesen: {path}");
	Console.WriteLine($"Zeilenanzahl: {lines.Length}");

    var banks = new List<BatteriesBank>();
    foreach (var line in lines)
    {
        var batteryValues = line.ToCharArray().Select(x => byte.Parse(x.ToString())).ToArray();
        banks.Add(new BatteriesBank(batteryValues));
    }

    return banks.ToArray();
}

static int GetMaxJoltage(BatteriesBank bank)
{
    byte first = bank.Batteries.Max();
    byte second = 0;
    int index = Array.IndexOf(bank.Batteries, first);
    if (index < bank.Batteries.Length - 1)
    {
        second = bank.Batteries[(index + 1)..].Max();
    }
    else
    {
        second = first;
        first = bank.Batteries[..(index)].Max();
    }
    return first*10+second;
}

static long GetMax12Joltage(BatteriesBank bank)
{
    var positions = new Dictionary<int, byte>();

    for (int i = 0; i < 12; i++)
    {
        var rightUnusedIndex = bank.Batteries.Length - 1;
        while (positions.ContainsKey(rightUnusedIndex))
        {
            rightUnusedIndex--;
        }
        var leftUnusedIndex = positions.Keys.Append(-1).Where(x => x <= rightUnusedIndex).Max() + 1;
        var slice = bank.Batteries[leftUnusedIndex..(rightUnusedIndex+1)];
        var maxValue = slice.Max();
        positions.Add(Array.IndexOf(slice, maxValue) + leftUnusedIndex, maxValue);
    }

    return long.Parse(
        string.Concat(
            positions.OrderBy(x => x.Key)
            .Select(x => x.Value.ToString())));
}