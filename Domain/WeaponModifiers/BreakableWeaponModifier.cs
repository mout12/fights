using System;

namespace fights;

public sealed class BreakableWeaponModifier : IRepairableWeaponModifier
{
    private readonly int _breakChance;
    private readonly string _brokenName;
    private readonly int _brokenDamage;
    private string? _originalName;
    private bool _isBroken;

    public BreakableWeaponModifier(int breakChance, string brokenName, int brokenDamage)
    {
        if (breakChance < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(breakChance), "Break chance must be at least 1.");
        }

        if (string.IsNullOrWhiteSpace(brokenName))
        {
            throw new ArgumentException("Broken weapon name must be specified.", nameof(brokenName));
        }

        if (brokenDamage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(brokenDamage), "Broken weapon damage must be at least 1.");
        }

        _breakChance = breakChance;
        _brokenName = brokenName;
        _brokenDamage = brokenDamage;
    }

    public void BeforeAttack(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);

        _originalName ??= weapon.TemplateName;

        if (_isBroken)
        {
            return;
        }

        if (Random.Shared.Next(0, _breakChance) != 0)
        {
            return;
        }

        _isBroken = true;
        weapon.UpdateState(_brokenName, _brokenDamage);
        Console.WriteLine($"{_originalName} shatters! It's now {_brokenName}.");
    }

    public IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        ArgumentNullException.ThrowIfNull(payload);
        return payload;
    }

    public IWeaponModifier Clone() => new BreakableWeaponModifier(_breakChance, _brokenName, _brokenDamage);

    public string? CaptureState(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        return _isBroken ? "broken" : null;
    }

    public void RestoreState(Weapon weapon, string? state)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        _originalName = weapon.TemplateName;
        _isBroken = string.Equals(state, "broken", StringComparison.OrdinalIgnoreCase);
    }

    public bool CanRepair(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        return _isBroken;
    }

    public bool TryRepair(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        if (!_isBroken)
        {
            return false;
        }

        _isBroken = false;
        weapon.ResetToBase();
        Console.WriteLine($"{weapon.TemplateName} has been repaired to its former glory.");
        return true;
    }
}
