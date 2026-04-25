using System;

namespace fights;

public sealed class MysteryBoxEncounter : IRandomEncounter
{
    private const int TrapChancePercent = 40;
    private const int MinimumGold = 25;
    private const int MaximumGoldExclusive = 101;

    public string Name => "Mysterious Box";

    public bool Execute(Player player, LevelContent level, IInputSelectionService inputSelector)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(level);
        ArgumentNullException.ThrowIfNull(inputSelector);

        Console.WriteLine("You stumble upon a mysterious box half-buried beside the path.");
        var shouldOpen = inputSelector.SelectOption("Do you want to open it?", new[]
        {
            new InputOption<bool>("[O]pen the box", true, Hotkey: 'o'),
            new InputOption<bool>("[L]eave it alone", false, Hotkey: 'l')
        });

        if (!shouldOpen)
        {
            Console.WriteLine("You leave the box untouched and return to town.");
            return true;
        }

        if (GameRandom.Current.Next(1, 101) <= TrapChancePercent)
        {
            Console.WriteLine("It's a trap! A powerful guardian springs from the shadows.");
            var guardian = CreateGuardian(level);
            var fight = new Fight(player, guardian, inputSelector);
            var playerSurvived = fight.Start();
            if (!playerSurvived)
            {
                return false;
            }

            if (guardian.Health > 0)
            {
                Console.WriteLine("You escape with your life, but the box snaps shut and sinks into the earth.");
                return true;
            }

            Console.WriteLine("With the guardian defeated, the box clicks open.");
        }
        else
        {
            Console.WriteLine("The lid opens without a sound.");
        }

        AwardGold(player);
        return true;
    }

    private static Fighter CreateGuardian(LevelContent level)
    {
        var template = level.Enemies[GameRandom.Current.Next(level.Enemies.Count)];
        return new Fighter(
            $"Empowered {template.Name}",
            checked(template.MaxHealth * 2),
            template.Weapon.Clone(),
            template.Armor,
            gold: 0);
    }

    private static void AwardGold(Player player)
    {
        var gold = (uint)GameRandom.Current.Next(MinimumGold, MaximumGoldExclusive);
        player.GainGold(gold);
        Console.WriteLine($"Inside the box, you find {gold} gold.");
    }
}
