namespace fights;

public interface IFighter
{
    string Name { get; }
    int Health { get; }
    IWeapon Weapon { get; }
    uint Gold { get; }
    void TakeDamage(int amount);
}
