using System;

namespace fights;

public sealed class DoubleStrikeWeaponModifier : IWeaponModifier
{
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

    public IWeaponModifier Clone() => new DoubleStrikeWeaponModifier();

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
