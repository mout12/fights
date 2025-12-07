using System;

namespace fights;

public class HealersHut
{
    private const uint HealCost = 100u;
    private readonly IInputSelectionService _inputSelector;

    public HealersHut(IInputSelectionService inputSelector)
    {
        _inputSelector = inputSelector ?? throw new ArgumentNullException(nameof(inputSelector));
    }

    public void Enter(Fighter fighter)
    {
        if (fighter is null)
        {
            throw new ArgumentNullException(nameof(fighter));
        }

        Console.WriteLine("You step into the healer's hut. The air smells of herbs and incense.");
        Console.WriteLine($"\"For {HealCost}g, I'll mend your wounds,\" the healer offers.");
        Console.WriteLine($"Your health: {fighter.Health}/{fighter.MaxHealth}. Your gold: {fighter.Gold}g.");

        if (fighter.Health >= fighter.MaxHealth)
        {
            Console.WriteLine("You're already at full strength. No healing needed today.");
            return;
        }

        var choice = _inputSelector.SelectOption("What will you do?", new[]
        {
            new InputOption<Action>(
                $"Pay {HealCost}g for a full heal",
                () =>
                {
                    if (!fighter.TrySpendGold(HealCost))
                    {
                        Console.WriteLine("You don't have enough gold. The healer shakes their head apologetically.");
                        return;
                    }

                    fighter.HealToFull();
                    Console.WriteLine("Warm light surrounds you as your wounds knit together. You're fully healed!");
                    Console.WriteLine($"Gold remaining: {fighter.Gold}g. Health: {fighter.Health}/{fighter.MaxHealth}.");
                }),
            new InputOption<Action>(
                "Leave without healing",
                () => Console.WriteLine("You leave without seeking the healer's aid."))
        });

        choice();
    }
}
