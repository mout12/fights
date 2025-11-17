namespace fights;

public interface IWeapon
{
    string Name { get; }
    int Damage { get; }
    IDamagePayload CreateDamagePayload();
}
