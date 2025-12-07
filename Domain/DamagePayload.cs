namespace fights;

public sealed class DamagePayload : IDamagePayload
{
    public DamagePayload(int damage, int selfDamage, bool isCritical)
    {
        Damage = damage;
        SelfDamage = selfDamage;
        IsCritical = isCritical;
    }

    public int Damage { get; }
    public int SelfDamage { get; }
    public bool IsCritical { get; }
}
