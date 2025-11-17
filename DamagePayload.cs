namespace fights;

public sealed class DamagePayload : IDamagePayload
{
    public DamagePayload(int damage, bool isCritical)
    {
        Damage = damage;
        IsCritical = isCritical;
    }

    public int Damage { get; }
    public bool IsCritical { get; }
}
