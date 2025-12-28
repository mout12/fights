using System;

namespace fights;

public class Fighter : IFighter
{
    private PoisonState? _poison;

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
    public PoisonState? ActivePoison => _poison;

    public int TakeDamage(IDamagePayload damagePayload)
    {
        damagePayload = damagePayload ?? throw new ArgumentNullException(nameof(damagePayload));

        var baseDamage = damagePayload.Damage;
        if (baseDamage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(damagePayload), "Damage must be non-negative.");
        }

        var mitigatedDamage = baseDamage;
        if (GameRandom.Current.Next(0, 2) == 0) // 50/50 chance to mitigate
        {
            mitigatedDamage = Math.Max(0, baseDamage - Armor.Defense);
        }

        ApplyDamage(mitigatedDamage);
        return mitigatedDamage;
    }

    public void TakeSelfDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        ApplyDamage(amount);
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

    public void ApplyPoison(PoisonState poison)
    {
        if (!poison.HasEffect)
        {
            return;
        }

        if (_poison is null || _poison.Value.RemainingTurns <= 0)
        {
            _poison = poison;
            return;
        }

        var existing = _poison.Value;
        var mergedDamage = Math.Max(existing.DamagePerTurn, poison.DamagePerTurn);
        _poison = new PoisonState(poison.TickChancePercent, mergedDamage, poison.RemainingTurns);
    }

    public PoisonTickResult TickPoison()
    {
        if (_poison is not { } poison || poison.RemainingTurns <= 0)
        {
            _poison = null;
            return PoisonTickResult.None;
        }

        var triggered = GameRandom.Current.Next(1, 101) <= poison.TickChancePercent;
        var damage = triggered ? poison.DamagePerTurn : 0;
        var remaining = poison.RemainingTurns - 1;
        _poison = remaining > 0 ? new PoisonState(poison.TickChancePercent, poison.DamagePerTurn, remaining) : null;

        return PoisonTickResult.FromTick(triggered, damage, remaining);
    }

    public void RestorePoison(PoisonState? poisonState)
    {
        if (poisonState is null || !poisonState.Value.HasEffect)
        {
            _poison = null;
            return;
        }

        _poison = poisonState;
    }

    private void ApplyDamage(int amount)
    {
        var updatedHealth = Health - amount;
        Health = updatedHealth < 0 ? 0 : updatedHealth;
    }
}
