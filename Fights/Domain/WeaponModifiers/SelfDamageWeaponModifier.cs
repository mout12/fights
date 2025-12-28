using System;

namespace fights;

public sealed class SelfDamageWeaponModifier : IWeaponModifier
{
    private readonly int _selfDamage;

    public SelfDamageWeaponModifier(int selfDamage)
    {
        if (selfDamage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(selfDamage), "Self damage must be non-negative.");
        }

        _selfDamage = selfDamage;
    }

    public void BeforeAttack(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
    }

    public IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        ArgumentNullException.ThrowIfNull(payload);

        if (_selfDamage == 0)
        {
            return payload;
        }

        if (payload is DamagePayload concrete)
        {
            return concrete.WithSelfDamage(payload.SelfDamage + _selfDamage);
        }

        return new DamagePayload(payload.Damage, payload.SelfDamage + _selfDamage, payload.IsCritical, payload.PoisonToApply);
    }

    public IWeaponModifier Clone() => new SelfDamageWeaponModifier(_selfDamage);

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
