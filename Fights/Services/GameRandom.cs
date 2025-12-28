using System;

namespace fights;

public static class GameRandom
{
    private static IRandomNumberGenerator _current = DefaultRandomNumberGenerator.Instance;

    public static IRandomNumberGenerator Current
    {
        get => _current;
        set => _current = value ?? throw new ArgumentNullException(nameof(value));
    }
}

public sealed class DefaultRandomNumberGenerator : IRandomNumberGenerator
{
    public static readonly DefaultRandomNumberGenerator Instance = new();

    private DefaultRandomNumberGenerator()
    {
    }

    public int Next(int maxExclusive) => Random.Shared.Next(maxExclusive);

    public int Next(int minInclusive, int maxExclusive) => Random.Shared.Next(minInclusive, maxExclusive);
}
