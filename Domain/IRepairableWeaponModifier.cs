namespace fights;

public interface IRepairableWeaponModifier : IWeaponModifier
{
    bool CanRepair(Weapon weapon);
    bool TryRepair(Weapon weapon);
}
