// not resolved the correct answer yet

using day8;

// expects two arguments: input file path and max connections to check
if (args.Length < 2)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file> <max-connections-to-check>");
	Environment.Exit(2);
}

var filePath = args[0];
var maxConnectionsToCheck = int.Parse(args[1]);

try
{
    var multipledJunctionBoxes = 1L;
    var junctionBoxs = ReadJunctionBoxesFromInputFile(filePath);
    var circuits = ConnectNearestJunctionBoxes(junctionBoxs, maxConnectionsToCheck)
        .OrderByDescending(c => c.JunctionBoxes.Count);
    Console.WriteLine($"Number of circuits: {circuits.Count()}");
    circuits.Take(3).ToList().ForEach(c =>
    {
        Console.WriteLine($"Circuit has {c.JunctionBoxes.Count} junction boxes.");
        multipledJunctionBoxes *= c.JunctionBoxes.Count;
    });

    var lastTwoXCoordinates = circuits.First().JunctionBoxes.Skip(circuits.First().JunctionBoxes.Count-2).Take(2).Select(box => box.Position.X).ToArray();
    Console.WriteLine($"Multiplied total of junction boxes in the three largest circuits: {multipledJunctionBoxes}");
    Console.WriteLine($"Last two X coordinates in the biggest circuit: {lastTwoXCoordinates[0]}, {lastTwoXCoordinates[1]} = {Convert.ToInt64(lastTwoXCoordinates[0]) * lastTwoXCoordinates[1]}");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Error reading/parsing the file: {ex.Message}");
	Environment.Exit(4);
}

static JunctionBox[] ReadJunctionBoxesFromInputFile(string path)
{
    var positions = File.ReadAllLines(path)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(x =>
        {
            var line = x.Split(',');
            return new Position(uint.Parse(line[0]), uint.Parse(line[1]), uint.Parse(line[2]));
        })
        .ToArray();
    Console.WriteLine($"File read: {path}");
    Console.WriteLine($"Number of positions: {positions.Length}");

    return positions.Select(pos => new JunctionBox(pos)).ToArray();
}

static Circuit[] ConnectNearestJunctionBoxes(JunctionBox[] junctionBoxes, int? maxConnectionsToCheck = null)
{
    if (junctionBoxes.Length == 0) return Array.Empty<Circuit>();

    maxConnectionsToCheck ??= junctionBoxes.Length;
    var circuits = junctionBoxes.Select(box => new Circuit(box)).ToList();
    var distances = GetAllDistances(junctionBoxes);

    for (int i = 1; i <= maxConnectionsToCheck && distances.Count > 0; i++)
    {
        var (boxA, boxB, distance) = distances.Dequeue();
        var circuitA = circuits.Single(c => c.JunctionBoxes.Contains(boxA));
        var circuitB = circuits.Single(c => c.JunctionBoxes.Contains(boxB));
        if (circuitA == circuitB)
        {
            boxA.ConnectedBoxes.Add(boxB);
            boxB.ConnectedBoxes.Add(boxA);
            // Console.WriteLine($"Skipping connection between ({boxA.Position.X}, {boxA.Position.Y}, {boxA.Position.Z}) and ({boxB.Position.X}, {boxB.Position.Y}, {boxB.Position.Z}) as they are already in the same circuit.");
            continue;
        }

        var newCircuit = circuitA.Combine(circuitB, boxB, boxA);
        circuits.Remove(circuitB);
        circuits.Remove(circuitA);
        circuits.Add(newCircuit);
        Console.WriteLine($"Connecting junction boxes at ({boxA.Position.X}, {boxA.Position.Y}, {boxA.Position.Z}) and ({boxB.Position.X}, {boxB.Position.Y}, {boxB.Position.Z}) with distance {distance:F2}. Number of junction boxes is now {newCircuit.JunctionBoxes.Count}. Number of circuits is now {circuits.Count}.");

        if (circuits.Count == 1)
        {
            Console.WriteLine("All junction boxes are now connected in a single circuit.");
            break;
        }
    }

    return circuits.ToArray();
}

// Returns a queue of distances between all junction boxes, sorted ascending by distance
static Queue<(JunctionBox boxA, JunctionBox boxB, double distance)> GetAllDistances(JunctionBox[] junctionBoxes)
{
    var distances = new List<(JunctionBox boxA, JunctionBox boxB, double distance)>();
    for (int i = 0; i < junctionBoxes.Length; i++)
    {
        for (int j = i + 1; j < junctionBoxes.Length; j++)
        {
            if (i == j) continue;
            var boxA = junctionBoxes[i];
            var boxB = junctionBoxes[j];
            var distance = boxA.Position.DistanceTo(boxB.Position);
            distances.Add((boxA, boxB, distance));
        }
    }
    return new Queue<(JunctionBox boxA, JunctionBox boxB, double distance)>(distances.OrderBy(t => t.distance));
}