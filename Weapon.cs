namespace fights;

public class Weapon : IWeapon
{
    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }

    public string Name { get; }
    public int Damage { get; }
}
