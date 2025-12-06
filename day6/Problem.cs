namespace day6;

record Problem(char Operator, long[] Values)
{
    public long Solve()
    {
        return Operator switch
        {
            '+' => Values.Sum(),
            '*' => Values.Aggregate(1L, (acc, val) => acc * val),
            _ => throw new InvalidOperationException($"Unbekannter Operator: {Operator}")
        };
    }

    public override string ToString()
    {
        return $"{string.Join($" {Operator} ", Values)}";
    }
}
