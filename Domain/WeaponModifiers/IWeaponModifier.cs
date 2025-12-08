namespace fights;

public interface IWeaponModifier
{
    void BeforeAttack(Weapon weapon);
    IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload);
    IWeaponModifier Clone();
    string? CaptureState(Weapon weapon);
    void RestoreState(Weapon weapon, string? state);
}
