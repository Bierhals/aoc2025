using System;
using System.Collections.ObjectModel;

namespace day8;

class Circuit
{
    readonly List<JunctionBox> _junctionBoxes;

    public Circuit(JunctionBox rootJunctionBox)
    {
        _junctionBoxes = new List<JunctionBox> { rootJunctionBox };
    }

    private Circuit(IEnumerable<JunctionBox> junctionBoxes)
    {
        _junctionBoxes = junctionBoxes.ToList();
    }

    public ReadOnlyCollection<JunctionBox> JunctionBoxes 
    {
        get
        {
            return _junctionBoxes.AsReadOnly();;
        }
    }

    public Circuit Combine(Circuit otherCircuit, JunctionBox otherCircuitBox, JunctionBox thisCircuitBox)
    {
        if (this == otherCircuit)
        {
            throw new ArgumentException("Cannot combine a circuit with itself.");
        }
        if (!_junctionBoxes.Contains(thisCircuitBox))
        {
            throw new ArgumentException("This circuit does not contain the specified junction box.");
        }
        if (!otherCircuit.JunctionBoxes.Contains(otherCircuitBox))
        {
            throw new ArgumentException("Other circuit does not contain the specified junction box.");
        }

        thisCircuitBox.ConnectedBoxes.Add(otherCircuitBox);
        otherCircuitBox.ConnectedBoxes.Add(thisCircuitBox);

        var newCiruit = new Circuit(this._junctionBoxes.Concat(otherCircuit.JunctionBoxes).Where(x => x != otherCircuitBox && x != thisCircuitBox).Concat(new[] { thisCircuitBox, otherCircuitBox }));
        return newCiruit;
    }
}
