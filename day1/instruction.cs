namespace day1;

record Instruction(char Direction, int Steps)
{
    public int GetDelta()
    {
        return Direction == 'R' ? Steps : -Steps;
    }
}