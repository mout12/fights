using System;
using System.Collections.Generic;
using System.Linq;

namespace fights;

public class Armorer
{
    private readonly List<(IArmor Armor, uint Cost)> _armorOffers;
    private readonly IInputSelectionService _inputSelector;

    public Armorer(IEnumerable<(IArmor armor, uint cost)> armorOffers, IInputSelectionService inputSelector)
    {
        if (armorOffers is null)
        {
            throw new ArgumentNullException(nameof(armorOffers));
        }

        _armorOffers = armorOffers.ToList();

        if (_armorOffers.Count == 0)
        {
            throw new ArgumentException("Shop must contain at least one armor offer.", nameof(armorOffers));
        }

        _inputSelector = inputSelector ?? throw new ArgumentNullException(nameof(inputSelector));
    }

    public void Enter(Fighter fighter)
    {
        if (fighter is null)
        {
            throw new ArgumentNullException(nameof(fighter));
        }

        Console.WriteLine("Welcome to the armorer's shop!");
        Console.WriteLine($"You've got {fighter.Gold}g to spend. Choose armor to wear into the fight:");

        const string prompt = "Select armor to purchase:";
        while (true)
        {
            var options = new List<InputOption<Func<bool>>>();
            foreach (var (armor, cost) in _armorOffers)
            {
                options.Add(new InputOption<Func<bool>>(
                    $"{armor.Name} (Defense: {armor.Defense}) - {cost}g",
                    () => TryPurchaseArmor(fighter, armor, cost)));
            }

            options.Add(new InputOption<Func<bool>>(
                "Leave without buying anything",
                () =>
                {
                    Console.WriteLine("You leave the armorer without buying anything.");
                    return false;
                }));

            var action = _inputSelector.SelectOption(prompt, options);
            var continueShopping = action();
            if (!continueShopping)
            {
                break;
            }
        }
    }

    private static bool TryPurchaseArmor(Fighter fighter, IArmor armor, uint cost)
    {
        if (!fighter.TrySpendGold(cost))
        {
            Console.WriteLine("Not enough gold. Pick something cheaper.");
            return true;
        }

        fighter.EquipArmor(armor);
        Console.WriteLine($"You purchased the {armor.Name}! Remaining gold: {fighter.Gold}g.");
        return false;
    }
}
