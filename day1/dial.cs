namespace day1;

class Dial
{
    private int position;
    private const int minPosition = 0;
    private readonly int maxPosition;
    private readonly int dialPositions;

    public Dial(int maxPosition, int initialPosition)
    {
        this.maxPosition = maxPosition;
        position = initialPosition;
        dialPositions = maxPosition - minPosition + 1;
    }

    // Rotiert das Ziffernblatt entsprechend der Anweisung und gibt die Anzahl der Nullen zurück, die dabei erreicht wurden
    public int Rotate(Instruction instruction)
    {
        // Verwende GetDelta statt Zugriff auf einzelne Properties
        var delta = instruction.GetDelta();
        return Rotate(delta);
    }

    // Rotiert das Ziffernblatt entsprechend der Anweisung und gibt die Anzahl der Nullen zurück, die dabei erreicht wurden
    public int Rotate(char direction, int steps)
    {
        var delta = direction == 'R' ? steps : -steps;
        return Rotate(delta);
    }

    // Rotiert das Ziffernblatt direkt mit einem signed Delta (R = +, L = -)
    private int Rotate(int delta)
    {
        var zeroCount = 0;

        if (delta > 0)
        {
            // Anzahl der Durchläufe über die Grenze nach rechts
            zeroCount = (position + delta) / dialPositions;
        }
        else if (delta < 0)
        {
            var deltaPositiv = -delta;
            if (position == 0)
            {
                // startet bei 0: dialPositions, 2-dialPositions, ... Schritte führen zu Treffern auf 0
                zeroCount = deltaPositiv / dialPositions;
            }
            else
            {
                // trifft auf 0, wenn deltaPositiv >= position (erstes Mal bei k = p), danach alle dialPositions-Schritte
                zeroCount = deltaPositiv >= position ? 1 + (deltaPositiv - position) / dialPositions : 0;
            }
        }

        // Neue Position normalisieren in Bereich [0, dialPositions-1]
        var raw = position + delta;
        position = ((raw % dialPositions) + dialPositions) % dialPositions;

        return zeroCount;
    }

    public int Position
    {
        get
        {
            return position;
        }
    }
}