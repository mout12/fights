namespace fights;

public class Fighter : IFighter
{
    public Fighter(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }

    public string Name { get; private set; }
    public int Health { get; private set; }
    public int Damage { get; private set; }
}
