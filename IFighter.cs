namespace fights;

public interface IFighter
{
    string Name { get; }
    int Health { get; }
    IWeapon Weapon { get; }
    void TakeDamage(int amount);
}
