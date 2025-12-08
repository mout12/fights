namespace fights;

public interface IWeapon
{
    string Name { get; }
    int Damage { get; }
    string TemplateName { get; }
    IDamagePayload CreateDamagePayload();
    WeaponState CaptureState();
    void RestoreState(WeaponState state);
    IWeapon Clone();
    bool CanRepair { get; }
    bool TryRepair();
}
