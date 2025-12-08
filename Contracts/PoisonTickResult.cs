namespace fights;

public readonly struct PoisonTickResult
{
    private PoisonTickResult(bool hadPoison, bool triggered, int damage, int remainingTurns)
    {
        HadPoison = hadPoison;
        Triggered = triggered;
        Damage = damage < 0 ? 0 : damage;
        RemainingTurns = remainingTurns < 0 ? 0 : remainingTurns;
    }

    public bool HadPoison { get; }
    public bool Triggered { get; }
    public int Damage { get; }
    public int RemainingTurns { get; }
    public bool Expired => HadPoison && RemainingTurns <= 0;

    public static PoisonTickResult FromTick(bool triggered, int damage, int remainingTurns)
        => new(true, triggered, damage, remainingTurns);

    public static PoisonTickResult None { get; } = new(false, false, 0, 0);
}
