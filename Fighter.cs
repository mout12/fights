using System;

namespace fights;

public class Fighter : IFighter
{
    public Fighter(string name, int health, IWeapon weapon, IArmor armor, uint gold)
    {
        Name = name;
        if (health <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(health), "Health must be positive.");
        }

        MaxHealth = health;
        Health = health;
        Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        Armor = armor ?? throw new ArgumentNullException(nameof(armor));
        Gold = gold;
    }

    public string Name { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public IWeapon Weapon { get; private set; }
    public IArmor Armor { get; private set; }
    public uint Gold { get; private set; }

    public int TakeDamage(IDamagePayload damagePayload)
    {
        damagePayload = damagePayload ?? throw new ArgumentNullException(nameof(damagePayload));

        var baseDamage = damagePayload.Damage;
        if (baseDamage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(damagePayload), "Damage must be non-negative.");
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

    public void GainGold(uint amount)
    {
        Gold = checked(Gold + amount);
    }

    public void HealToFull()
    {
        Health = MaxHealth;
    }

    public void RestoreHealth(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxHealth), "Max health must be positive.");
        }

        if (currentHealth < 0 || currentHealth > maxHealth)
        {
            throw new ArgumentOutOfRangeException(nameof(currentHealth), "Health must be within 0 and max health.");
        }

        MaxHealth = maxHealth;
        Health = currentHealth;
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
