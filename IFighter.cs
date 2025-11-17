namespace fights;

public interface IFighter
{
    string Name { get; }
    int Health { get; }
    IWeapon Weapon { get; }
    IArmor Armor { get; }
    uint Gold { get; }
    int TakeDamage(IDamagePayload damagePayload);
}
