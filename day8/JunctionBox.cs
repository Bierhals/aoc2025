namespace day8;

class JunctionBox
{
    public JunctionBox(Position position)
    {
        Position = position;
        ConnectedBoxes = new List<JunctionBox>();
    }

    public Position Position { get; }
    public List<JunctionBox> ConnectedBoxes { get; set; }
}