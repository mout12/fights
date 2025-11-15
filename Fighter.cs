using System;

namespace fights;

public class Fighter : IFighter
{
    public Fighter(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }

    public string Name { get; private set; }
    public int Health { get; private set; }
    public int Damage { get; private set; }

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
