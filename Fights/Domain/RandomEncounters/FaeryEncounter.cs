using System;

namespace fights;

public sealed class FaeryEncounter : IRandomEncounter
{
    public string Name => "Faery";

    public bool Execute(Player player, LevelContent level, IInputSelectionService inputSelector)
    {
        ArgumentNullException.ThrowIfNull(player);

        Console.WriteLine("A faery flutters out of the tall grass, scattering warm light around you.");
        player.HealToFull();
        Console.WriteLine($"The faery mends your wounds before vanishing. Health restored to {player.Health}/{player.MaxHealth}.");
        return true;
    }
}
