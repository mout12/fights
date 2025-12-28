using System;

namespace fights;

public sealed class PoisonWeaponModifier : IWeaponModifier
{
    private readonly int _applyChancePercent;
    private readonly PoisonState _poisonTemplate;

    public PoisonWeaponModifier(int applyChancePercent, int tickChancePercent, int damagePerTurn, int durationTurns)
    {
        if (applyChancePercent < 1 || applyChancePercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(applyChancePercent), "Apply chance must be between 1 and 100.");
        }

        _poisonTemplate = new PoisonState(tickChancePercent, damagePerTurn, durationTurns);
        _applyChancePercent = applyChancePercent;
    }

    public void BeforeAttack(Weapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
    }

    public IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        ArgumentNullException.ThrowIfNull(payload);

        if (!_poisonTemplate.HasEffect)
        {
            return payload;
        }

        if (GameRandom.Current.Next(1, 101) > _applyChancePercent)
        {
            return payload;
        }

        var poisonCopy = new PoisonState(_poisonTemplate.TickChancePercent, _poisonTemplate.DamagePerTurn, _poisonTemplate.RemainingTurns);
        if (payload is DamagePayload concrete)
        {
            return concrete.WithPoison(poisonCopy);
        }

        return new DamagePayload(payload.Damage, payload.SelfDamage, payload.IsCritical, poisonCopy);
    }

    public IWeaponModifier Clone() => new PoisonWeaponModifier(_applyChancePercent, _poisonTemplate.TickChancePercent, _poisonTemplate.DamagePerTurn, _poisonTemplate.RemainingTurns);

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
