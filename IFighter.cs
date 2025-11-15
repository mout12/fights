namespace fights;

public interface IFighter
{
    string Name { get; }
    int Health { get; }
    int Damage { get; }
    void TakeDamage(int amount);
}
