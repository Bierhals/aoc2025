// Erwartet: 1 Argument = Pfad zur Eingabedatei
using day5;

if (args.Length < 1)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
	Environment.Exit(2);
}

var filePath = args[0];

try
{
    var (freshIngredients, ingredients) = ReadIngredientsFromInputFile(filePath);
    Console.WriteLine($"Fresh ingridient ranges: {freshIngredients.Count}");
    Console.WriteLine($"Amount of ingredients: {ingredients.Count}");
    var freshCount = 0;

    foreach (var ingredient in ingredients)
    {
        if (freshIngredients.Any(range => range.IsInRange(ingredient)))
        {
            Console.WriteLine($"Ingredient {ingredient} is fresh");
            freshCount++;
        }
    }

    var freshCountOverall = CountFreshIngredients(freshIngredients);
    
    Console.WriteLine($"{freshCount} ingredients are fresh.");
    Console.WriteLine($"Overall, {freshCountOverall} ingredients are fresh in the given ranges.");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
	Environment.Exit(4);
}

static (List<IngrediantsRange> freshIngredients, List<long> ingredients) ReadIngredientsFromInputFile(string path)
{
    var lines = File.ReadAllLines(path);
    Console.WriteLine($"Datei gelesen: {path}");
    Console.WriteLine($"Zeilenanzahl: {lines.Length}");

    var freshIngredients = new List<IngrediantsRange>();
    var ingredients = new List<long>();

    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        if (line.Contains('-'))
        {
            var parts = line.Split('-');    
            var freshRange = new IngrediantsRange(long.Parse(parts[0]), long.Parse(parts[1]));
            freshIngredients.Add(freshRange);
        }
        else
        {
            ingredients.Add(long.Parse(line));
        }
    }

    return (freshIngredients, ingredients);
}

static long CountFreshIngredients(List<IngrediantsRange> freshIngredients)
{
    long freshCount = 0;

    var orderedRanges = freshIngredients.OrderBy(r => r.Min).ToList();
    long lastId = 0;

    foreach (var range in orderedRanges)
    {
        if (range.Max <= lastId)
        {
            continue;
        }
        var min = range.Min;
        var max = range.Max;
        if (min <= lastId)
        {
            min = lastId + 1;
        }

        freshCount += (max - min + 1);
        lastId = max;
    }

    return freshCount;
}