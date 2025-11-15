using System;

namespace fights;

public class Fighter : IFighter
{
    public Fighter(string name, int health, IWeapon weapon, uint gold)
    {
        Name = name;
        Health = health;
        Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        Gold = gold;
    }

    public string Name { get; private set; }
    public int Health { get; private set; }
    public IWeapon Weapon { get; private set; }
    public uint Gold { get; private set; }

    public void TakeDamage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Damage must be non-negative.");
        }

        var updatedHealth = Health - amount;
        Health = updatedHealth < 0 ? 0 : updatedHealth;
    }

    public bool TrySpendGold(uint amount)
    {
        if (Gold < amount)
        {
            return false;
        }

        Gold -= amount;
        return true;
    }

    public void EquipWeapon(IWeapon weapon)
    {
        Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
    }
}
