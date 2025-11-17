namespace fights;

public interface IDamagePayload
{
    int Damage { get; }
    bool IsCritical { get; }
}
