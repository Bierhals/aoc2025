namespace day5;

record IngrediantsRange(long Min, long Max)
{
    public bool IsInRange(long value)
    {
        return value >= Min && value <= Max;
    }
}