namespace fights;

public interface IRandomEncounter
{
    string Name { get; }
    void Execute(Player player);
}
