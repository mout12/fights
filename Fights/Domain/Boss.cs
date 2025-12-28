using System;

namespace fights;

public class Boss : Fighter, ILevel
{
    public Boss(string name, int level, int health, IWeapon weapon, IArmor armor, uint gold)
        : base(name, health, weapon, armor, gold)
    {
        if (level < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
        }

        Level = level;
    }

    public int Level { get; private set; }
}
