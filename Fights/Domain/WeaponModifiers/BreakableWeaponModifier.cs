using System;

namespace fights;

public sealed class BreakableWeaponModifier : IWeaponModifier
{
    private readonly int _breakChance;
    private readonly string _brokenWeaponKey;

    public BreakableWeaponModifier(int breakChance, string brokenWeaponKey)
    {
        if (breakChance < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(breakChance), "Break chance must be at least 1.");
        }

        if (string.IsNullOrWhiteSpace(brokenWeaponKey))
        {
            throw new ArgumentException("Broken weapon key must be specified.", nameof(brokenWeaponKey));
        }

        _breakChance = breakChance;
        _brokenWeaponKey = brokenWeaponKey;
    }

    public void BeforeAttack(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);

        if (GameRandom.Current.Next(0, _breakChance) != 0)
        {
            return;
        }

        var originalName = weapon.Name;
        if (weapon.TryReplaceWith(_brokenWeaponKey))
        {
            Console.WriteLine($"{originalName} shatters! It's now {weapon.Name}.");
        }
        else
        {
            Console.WriteLine($"{originalName} shatters beyond repair.");
        }
    }

    public IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        ArgumentNullException.ThrowIfNull(payload);
        return payload;
    }

    public IWeaponModifier Clone() => new BreakableWeaponModifier(_breakChance, _brokenWeaponKey);

    public string? CaptureState(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        return null;
    }

    public void RestoreState(Weapon weapon, string? state)
    {
        ArgumentNullException.ThrowIfNull(weapon);
    }
}
