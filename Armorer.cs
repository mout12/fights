using System;
using System.Collections.Generic;
using System.Linq;

namespace fights;

public class Armorer
{
    private readonly List<(IArmor Armor, uint Cost)> _armorOffers;

    public Armorer(IEnumerable<(IArmor armor, uint cost)> armorOffers)
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
    }

    public void Enter(Fighter fighter)
    {
        if (fighter is null)
        {
            throw new ArgumentNullException(nameof(fighter));
        }

        Console.WriteLine("Welcome to the armorer's shop!");
        Console.WriteLine($"You've got {fighter.Gold}g to spend. Choose armor to wear into the fight:");

        for (var i = 0; i < _armorOffers.Count; i++)
        {
            var (armor, cost) = _armorOffers[i];
            Console.WriteLine($"{i + 1}. {armor.Name} (Defense: {armor.Defense}) - {cost}g");
        }

        while (true)
        {
            Console.Write("Enter armor number to purchase or press Enter to keep your current armor: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"You keep your {fighter.Armor.Name} and {fighter.Gold}g.");
                break;
            }

            if (!int.TryParse(input, out var choice) || choice < 1 || choice > _armorOffers.Count)
            {
                Console.WriteLine("Invalid selection. Choose a number from the list.");
                continue;
            }

            var (selectedArmor, cost) = _armorOffers[choice - 1];

            if (!fighter.TrySpendGold(cost))
            {
                Console.WriteLine("Not enough gold. Pick something cheaper.");
                continue;
            }

            fighter.EquipArmor(selectedArmor);
            Console.WriteLine($"You purchased the {selectedArmor.Name}! Remaining gold: {fighter.Gold}g.");
            break;
        }
    }
}
