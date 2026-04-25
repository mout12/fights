namespace fights;

public interface IRandomEncounter
{
    string Name { get; }
    bool Execute(Player player, LevelContent level, IInputSelectionService inputSelector);
}
