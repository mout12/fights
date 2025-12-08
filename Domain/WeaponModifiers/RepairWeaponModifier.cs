using System;

namespace fights;

public sealed class RepairWeaponModifier : IRepairableWeaponModifier
{
    private readonly string _targetWeaponKey;

    public RepairWeaponModifier(string targetWeaponKey)
    {
        if (string.IsNullOrWhiteSpace(targetWeaponKey))
        {
            throw new ArgumentException("Repair target must be specified.", nameof(targetWeaponKey));
        }

        _targetWeaponKey = targetWeaponKey;
    }

    public void BeforeAttack(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
    }

    public IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        ArgumentNullException.ThrowIfNull(payload);
        return payload;
    }

    public IWeaponModifier Clone() => new RepairWeaponModifier(_targetWeaponKey);

    public string? CaptureState(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        return null;
    }

    public void RestoreState(Weapon weapon, string? state)
    {
        ArgumentNullException.ThrowIfNull(weapon);
    }

    public bool CanRepair(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        return true;
    }

    public bool TryRepair(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);

        var brokenName = weapon.Name;
        if (weapon.TryReplaceWith(_targetWeaponKey))
        {
            Console.WriteLine($"{brokenName} has been reforged into {weapon.Name}.");
            return true;
        }

        Console.WriteLine($"Failed to repair {brokenName}.");
        return false;
    }
}
