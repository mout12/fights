using System.Collections.Generic;

namespace fights;

public interface IWeapon
{
    string Name { get; }
    int Damage { get; }
    string TemplateName { get; }
    IList<IWeaponModifier> Modifiers { get; }
    IDamagePayload CreateDamagePayload();
    WeaponState CaptureState();
    void RestoreState(WeaponState state);
    IWeapon Clone();
}
