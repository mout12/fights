using System;

namespace fights;

public class HealersHut
{
    private const uint HealCost = 100u;
    private const uint CurePoisonCost = 75u;
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
        Console.WriteLine($"\"For {HealCost}g I can mend your wounds. For {CurePoisonCost}g I'll purge any toxins,\" the healer offers.");
        var poisonState = fighter.ActivePoison;
        var poisonStatus = poisonState is { } active && active.HasEffect
            ? $"Poisoned ({active.DamagePerTurn} dmg, {active.RemainingTurns} turns, {active.TickChancePercent}% chance)"
            : "Healthy";
        Console.WriteLine($"Your health: {fighter.Health}/{fighter.MaxHealth}. Status: {poisonStatus}. Your gold: {fighter.Gold}g.");

        var choice = _inputSelector.SelectOption("What will you do?", new[]
        {
            new InputOption<Action>(
                $"Pay {HealCost}g for a full heal",
                () =>
                {
                    if (fighter.Health >= fighter.MaxHealth)
                    {
                        Console.WriteLine("You're already at full strength. The healer smiles knowingly.");
                        PauseBeforeLeaving();
                        return;
                    }

                    if (!fighter.TrySpendGold(HealCost))
                    {
                        Console.WriteLine("You don't have enough gold. The healer shakes their head apologetically.");
                        PauseBeforeLeaving();
                        return;
                    }

                    fighter.HealToFull();
                    Console.WriteLine("Warm light surrounds you as your wounds knit together. You're fully healed!");
                    Console.WriteLine($"Gold remaining: {fighter.Gold}g. Health: {fighter.Health}/{fighter.MaxHealth}.");
                    PauseBeforeLeaving();
                }),
            new InputOption<Action>(
                $"Pay {CurePoisonCost}g to cure poison",
                () =>
                {
                    if (fighter.ActivePoison is not { } poison || !poison.HasEffect)
                    {
                        Console.WriteLine("The healer checks you over and shrugs. \"There's no poison to purge.\"");
                        PauseBeforeLeaving();
                        return;
                    }

                    if (!fighter.TrySpendGold(CurePoisonCost))
                    {
                        Console.WriteLine("You don't have enough gold for the antidote.");
                        PauseBeforeLeaving();
                        return;
                    }

                    fighter.RestorePoison(null);
                    Console.WriteLine("A bitter draught burns down your throat, and the poison fades away.");
                    Console.WriteLine($"Gold remaining: {fighter.Gold}g.");
                    PauseBeforeLeaving();
                }),
            new InputOption<Action>(
                "Leave without treatment",
                () =>
                {
                    Console.WriteLine("You leave without seeking the healer's aid.");
                    PauseBeforeLeaving();
                })
        });

        choice();
    }

    private static void PauseBeforeLeaving()
    {
        Console.WriteLine("Press any key to return to town...");
        Console.ReadKey(intercept: true);
        Console.WriteLine();
    }
}
