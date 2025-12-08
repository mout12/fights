namespace fights;

public class Weapon : IWeapon
{
    public Weapon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }

    public string Name { get; protected set; }
    public int Damage { get; protected set; }

    public virtual IDamagePayload CreateDamagePayload()
    {
        var isCritical = Random.Shared.Next(0, 10) == 0; // 10% chance
        var damage = isCritical ? Damage * 2 : Damage;
        return new DamagePayload(damage, selfDamage: 0, isCritical);
    }
}
