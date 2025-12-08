namespace fights;

public sealed class DamagePayload : IDamagePayload
{
    public DamagePayload(int damage, int selfDamage, bool isCritical, PoisonState? poisonToApply = null)
    {
        Damage = damage;
        SelfDamage = selfDamage;
        IsCritical = isCritical;
        PoisonToApply = poisonToApply;
    }

    public int Damage { get; }
    public int SelfDamage { get; }
    public bool IsCritical { get; }
    public PoisonState? PoisonToApply { get; }

    public DamagePayload WithSelfDamage(int selfDamage)
        => new(Damage, selfDamage, IsCritical, PoisonToApply);

    public DamagePayload WithPoison(PoisonState? poison)
        => new(Damage, SelfDamage, IsCritical, poison);
}
