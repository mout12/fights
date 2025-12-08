using System;
using System.Collections.Generic;
using System.Linq;

namespace fights;

public class Weapon : IWeapon
{
    private readonly List<IWeaponModifier> _modifiers;
    private readonly int _baseDamage;

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

        TemplateName = name;
        Name = name;
        Damage = damage;
        _baseDamage = damage;
        _modifiers = modifiers is null ? new List<IWeaponModifier>() : new List<IWeaponModifier>(modifiers);
    }

    public string TemplateName { get; }
    public string Name { get; private set; }
    public int Damage { get; private set; }
    public bool CanRepair => _modifiers.OfType<IRepairableWeaponModifier>().Any(m => m.CanRepair(this));

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

    internal void ResetToBase()
    {
        Name = TemplateName;
        Damage = _baseDamage;
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

    public bool TryRepair()
    {
        var repaired = false;
        foreach (var modifier in _modifiers.OfType<IRepairableWeaponModifier>())
        {
            if (modifier.TryRepair(this))
            {
                repaired = true;
            }
        }

        return repaired;
    }

    public WeaponState CaptureState()
    {
        var modifierStates = new List<string?>(_modifiers.Count);
        foreach (var modifier in _modifiers)
        {
            modifierStates.Add(modifier.CaptureState(this));
        }

        return new WeaponState(TemplateName, Name, Damage, modifierStates);
    }

    public void RestoreState(WeaponState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!string.Equals(state.TemplateName, TemplateName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Cannot restore state for '{TemplateName}' from template '{state.TemplateName}'.");
        }

        UpdateState(state.Name, state.Damage);

        var modifierCount = _modifiers.Count;
        for (var i = 0; i < modifierCount; i++)
        {
            var modifierState = i < state.ModifierStates.Count ? state.ModifierStates[i] : null;
            _modifiers[i].RestoreState(this, modifierState);
        }
    }

    public IWeapon Clone()
    {
        var clonedModifiers = new List<IWeaponModifier>(_modifiers.Count);
        foreach (var modifier in _modifiers)
        {
            clonedModifiers.Add(modifier.Clone());
        }

        var clone = new Weapon(TemplateName, _baseDamage, clonedModifiers);
        var state = CaptureState();
        clone.RestoreState(state);
        return clone;
    }
}
