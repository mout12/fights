namespace fights;

public interface IRandomNumberGenerator
{
    int Next(int maxExclusive);
    int Next(int minInclusive, int maxExclusive);
}
