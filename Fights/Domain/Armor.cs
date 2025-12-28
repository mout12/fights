namespace fights;

public class Armor : IArmor
{
    public Armor(string name, int defense)
    {
        Name = name;
        Defense = defense;
    }

    public string Name { get; }
    public int Defense { get; }
}
