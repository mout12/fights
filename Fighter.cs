using System;

namespace fights;

public class Fighter : IFighter
{
    public Fighter(string name, int health, IWeapon weapon, IArmor armor, uint gold)
    {
        Name = name;
        Health = health;
        Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        Armor = armor ?? throw new ArgumentNullException(nameof(armor));
        Gold = gold;
    }

    public string Name { get; private set; }
    public int Health { get; private set; }
    public IWeapon Weapon { get; private set; }
    public IArmor Armor { get; private set; }
    public uint Gold { get; private set; }

    public int TakeDamage(IWeapon weapon)
    {
        weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));

        var baseDamage = weapon.Damage;
        if (baseDamage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weapon), "Damage must be non-negative.");
        }

        var mitigatedDamage = baseDamage;
        if (Random.Shared.Next(0, 2) == 0) // 50/50 chance to mitigate
        {
            mitigatedDamage = Math.Max(0, baseDamage - Armor.Defense);
        }

        var updatedHealth = Health - mitigatedDamage;
        Health = updatedHealth < 0 ? 0 : updatedHealth;

        return mitigatedDamage;
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

    public void EquipArmor(IArmor armor)
    {
        Armor = armor ?? throw new ArgumentNullException(nameof(armor));
    }
}
