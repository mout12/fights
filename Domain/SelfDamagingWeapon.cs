using System;

namespace fights;

public class SelfDamagingWeapon : Weapon
{
    private readonly int _selfDamage;

    public SelfDamagingWeapon(string name, int damage, int selfDamage)
        : base(name, damage)
    {
        if (selfDamage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(selfDamage), "Self damage must be non-negative.");
        }

        _selfDamage = selfDamage;
    }

    public override IDamagePayload CreateDamagePayload()
    {
        var payload = base.CreateDamagePayload();
        return new DamagePayload(payload.Damage, _selfDamage, payload.IsCritical);
    }
}
