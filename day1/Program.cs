using System;
using System.IO;
using System.Collections.Generic;
using day1;

// Erwartet: 1 Argument = Pfad zur Eingabedatei
if (args.Length < 1)
{
	Console.Error.WriteLine("Usage: dotnet run -- <input-file>");
	Environment.Exit(2);
}

var filePath = args[0];

try
{
	// Instruktionen einlesen und parsen
	Instruction[] instructions = ReadInstructionsFromInputFile(filePath);

    var dial = new Dial(99, 50);
    var positions = new List<int> { dial.Position };
    var zeroPositions = new List<int> { dial.Position == 0 ? 1 : 0 };
    Console.WriteLine($"The dial starts by pointing at {dial.Position}");

    // Alle Instruktionen ausführen
    foreach (var instruction in instructions)
    {
        zeroPositions.Add(dial.Rotate(instruction));
        positions.Add(dial.Position);
        Console.Write($"The dial is rotated {instruction.Direction}{instruction.Steps} to point at {dial.Position}");
        if (zeroPositions[^1] > 0)
        {
            Console.Write($"; during this rotation, it points at 0 {zeroPositions[^1]} time(s)");
        }
        Console.WriteLine();
    }

    var passwordFinalZero = positions.Count(x => x == 0);
    var passwordTotalZero = zeroPositions.Sum();
    Console.WriteLine($"The password is {passwordFinalZero}");
    Console.WriteLine($"method 0x434C49434B: The password is {passwordTotalZero}");
}
catch (Exception ex)
{
	Console.Error.WriteLine($"Fehler beim Einlesen/Parsen der Datei: {ex.Message}");
	Environment.Exit(4);
}

static Instruction[] ReadInstructionsFromInputFile(string path)
{
    var lines = ReadInputFile(path);
	Console.WriteLine($"Datei gelesen: {path}");
	Console.WriteLine($"Zeilenanzahl: {lines.Length}");

    var instructions = ParseInstructions(lines);
	Console.WriteLine($"Anweisungen: {instructions.Length}");


    return instructions;
}

// Ausgelagerte Funktion für Dateilogik: Prüfung und Einlesen
static string[] ReadInputFile(string path)
{
	if (!File.Exists(path))
	{
		throw new FileNotFoundException("Datei nicht gefunden.", path);
	}

	return File.ReadAllLines(path);
}

// Wandelt alle Zeilen in Instruction-Records um; überspringt leere Zeilen
static Instruction[] ParseInstructions(string[] lines)
{
	var list = new List<Instruction>();
	foreach (var line in lines)
	{
		if (string.IsNullOrWhiteSpace(line))
			continue;

		list.Add(ParseInstruction(line));
	}

	return list.ToArray();
}

// Parst eine einzelne Zeile in eine Instruction.
// Erwartete Formate: "R4", "R 4", "R,4" (R oder L am Zeilenanfang, danach eine Zahl)
static Instruction ParseInstruction(string line)
{
	if (string.IsNullOrWhiteSpace(line))
		throw new FormatException("Leere Eingabezeile.");

	var trimmed = line.Trim();
	char dir = char.ToUpperInvariant(trimmed[0]);
	if (dir != 'R' && dir != 'L')
		throw new FormatException($"Ungültige Richtung '{dir}' in Zeile: {line}");

	string rest = trimmed.Substring(1).Trim();
	if (rest.StartsWith(",") || rest.StartsWith(":"))
		rest = rest.Substring(1).Trim();

	if (!int.TryParse(rest, out int steps))
		throw new FormatException($"Ungültige Schrittzahl in Zeile: {line}");

	return new Instruction(dir, steps);
}
