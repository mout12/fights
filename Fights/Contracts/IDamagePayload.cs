namespace fights;

public interface IDamagePayload
{
    int Damage { get; }
    int SelfDamage { get; }
    bool IsCritical { get; }
    PoisonState? PoisonToApply { get; }
}
