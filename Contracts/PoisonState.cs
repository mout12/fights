using System;

namespace fights;

public readonly struct PoisonState
{
    public PoisonState(int tickChancePercent, int damagePerTurn, int remainingTurns)
    {
        if (tickChancePercent < 0 || tickChancePercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(tickChancePercent), "Tick chance must be between 0 and 100.");
        }

        if (damagePerTurn < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(damagePerTurn), "Damage per turn must be non-negative.");
        }

        if (remainingTurns < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(remainingTurns), "Remaining turns must be non-negative.");
        }

        TickChancePercent = tickChancePercent;
        DamagePerTurn = damagePerTurn;
        RemainingTurns = remainingTurns;
    }

    public int TickChancePercent { get; }
    public int DamagePerTurn { get; }
    public int RemainingTurns { get; }

    public bool HasEffect => TickChancePercent > 0 && DamagePerTurn > 0 && RemainingTurns > 0;
}
