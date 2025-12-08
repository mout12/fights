namespace fights;

public interface IWeaponModifier
{
    void BeforeAttack(Weapon weapon);
    IDamagePayload ModifyPayload(Weapon weapon, IDamagePayload payload);
}
