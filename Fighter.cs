using System;

namespace fights;

public class Fighter : IFighter
{
    public Fighter(string name, int health, IWeapon weapon)
    {
        Name = name;
        Health = health;
        Weapon = weapon;
    }

    public string Name { get; private set; }
    public int Health { get; private set; }
    public IWeapon Weapon { get; private set; }

    public void TakeDamage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Damage must be non-negative.");
        }

        var updatedHealth = Health - amount;
        Health = updatedHealth < 0 ? 0 : updatedHealth;
    }
}
