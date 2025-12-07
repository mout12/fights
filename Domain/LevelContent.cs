using System;
using System.Collections.Generic;

namespace fights;

public class LevelContent
{
    public LevelContent(List<Fighter> enemies, Boss boss)
    {
        Enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));
        if (Enemies.Count == 0)
        {
            throw new ArgumentException("Each level must have at least one enemy.", nameof(enemies));
        }

        Boss = boss ?? throw new ArgumentNullException(nameof(boss));
    }

    public List<Fighter> Enemies { get; }
    public Boss Boss { get; }
}
