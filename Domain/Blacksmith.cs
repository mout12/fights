using System;
using System.Collections.Generic;
using System.Linq;

namespace fights;

public class Blacksmith
{
    private const uint RepairCost = 80;
    private readonly List<(IWeapon Weapon, uint Cost)> _weaponOffers;
    private readonly IInputSelectionService _inputSelector;

    public Blacksmith(IEnumerable<(IWeapon weapon, uint cost)> weaponOffers, IInputSelectionService inputSelector)
    {
        if (weaponOffers is null)
        {
            throw new ArgumentNullException(nameof(weaponOffers));
        }

        _weaponOffers = weaponOffers.ToList();

        if (_weaponOffers.Count == 0)
        {
            throw new ArgumentException("Shop must contain at least one weapon offer.", nameof(weaponOffers));
        }

        _inputSelector = inputSelector ?? throw new ArgumentNullException(nameof(inputSelector));
    }

    public void Enter(Fighter fighter)
    {
        if (fighter is null)
        {
            throw new ArgumentNullException(nameof(fighter));
        }

        Console.WriteLine("Welcome to the blacksmith's forge!");
        Console.WriteLine($"You've got {fighter.Gold}g to spend. Choose a weapon to take into the fight:");

        var description = "Select a weapon to purchase:";
        while (true)
        {
            var options = new List<InputOption<Func<bool>>>();
            foreach (var (weapon, cost) in _weaponOffers)
            {
                options.Add(new InputOption<Func<bool>>(
                    $"{weapon.Name} (Damage: {weapon.Damage}) - {cost}g",
                    () => TryPurchaseWeapon(fighter, weapon, cost)));
            }

            if (fighter.Weapon.CanRepair)
            {
                options.Add(new InputOption<Func<bool>>(
                    $"Repair your {fighter.Weapon.Name} ({RepairCost}g)",
                    () => TryRepairWeapon(fighter)));
            }

            options.Add(new InputOption<Func<bool>>(
                "Leave without buying anything",
                () =>
                {
                    Console.WriteLine("You leave the blacksmith without buying anything.");
                    return false;
                }));

            var action = _inputSelector.SelectOption(description, options);
            var continueShopping = action();
            if (!continueShopping)
            {
                PauseBeforeLeaving();
                break;
            }
        }
    }

    private static bool TryPurchaseWeapon(Fighter fighter, IWeapon weapon, uint cost)
    {
        if (!fighter.TrySpendGold(cost))
        {
            Console.WriteLine("Not enough gold. Pick something cheaper.");
            return true;
        }

        var purchasedWeapon = weapon.Clone();
        fighter.EquipWeapon(purchasedWeapon);
        Console.WriteLine($"You purchased the {purchasedWeapon.Name}! Remaining gold: {fighter.Gold}g.");
        return false;
    }

    private bool TryRepairWeapon(Fighter fighter)
    {
        if (!fighter.Weapon.CanRepair)
        {
            Console.WriteLine("Your weapon is already in perfect condition.");
            return true;
        }

        if (!fighter.TrySpendGold(RepairCost))
        {
            Console.WriteLine("Not enough gold to pay for repairs.");
            return true;
        }

        if (fighter.Weapon.TryRepair())
        {
            Console.WriteLine($"The blacksmith restores your {fighter.Weapon.Name}! Remaining gold: {fighter.Gold}g.");
        }
        else
        {
            Console.WriteLine("The blacksmith couldn't repair your weapon.");
        }

        return true;
    }

    private static void PauseBeforeLeaving()
    {
        Console.WriteLine("Press any key to return to town...");
        Console.ReadKey(intercept: true);
        Console.WriteLine();
    }
}
