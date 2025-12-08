using System;
using System.Collections.Generic;

namespace fights;

public class Weapon : IWeapon
{
    private readonly List<IWeaponModifier> _modifiers;

    public Weapon(string name, int damage, IEnumerable<IWeaponModifier>? modifiers = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Weapon name must be provided.", nameof(name));
        }

        if (damage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(damage), "Weapon damage must be non-negative.");
        }

        Name = name;
        Damage = damage;
        _modifiers = modifiers is null ? new List<IWeaponModifier>() : new List<IWeaponModifier>(modifiers);
    }

    public string Name { get; private set; }
    public int Damage { get; private set; }

    internal void UpdateState(string? newName = null, int? newDamage = null)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            Name = newName;
        }

        if (newDamage.HasValue)
        {
            if (newDamage.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newDamage), "Weapon damage must be non-negative.");
            }

            Damage = newDamage.Value;
        }
    }

    public virtual IDamagePayload CreateDamagePayload()
    {
        foreach (var modifier in _modifiers)
        {
            modifier.BeforeAttack(this);
        }

        var isCritical = Random.Shared.Next(0, 10) == 0; // 10% chance
        var damage = isCritical ? Damage * 2 : Damage;
        IDamagePayload payload = new DamagePayload(damage, selfDamage: 0, isCritical);

        foreach (var modifier in _modifiers)
        {
            payload = modifier.ModifyPayload(this, payload);
        }

        return payload;
    }
}
