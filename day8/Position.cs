namespace day8;

record Position(uint X, uint Y, uint Z)
{
    public double DistanceTo(Position other)
    {
        long deltaX = Convert.ToInt64(other.X) - X;
        long deltaY = Convert.ToInt64(other.Y) - Y;
        long deltaZ = Convert.ToInt64(other.Z) - Z;
        return Math.Sqrt(deltaX*deltaX + deltaY*deltaY + deltaZ*deltaZ);
    }
}
