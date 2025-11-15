using System;
using System.Collections.Generic;
using System.Linq;

namespace fights;

public class Shop
{
    private readonly List<(IWeapon Weapon, uint Cost)> _weaponOffers;

    public Shop(IEnumerable<(IWeapon weapon, uint cost)> weaponOffers)
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
    }

    public void Enter(Fighter fighter)
    {
        if (fighter is null)
        {
            throw new ArgumentNullException(nameof(fighter));
        }

        Console.WriteLine("Welcome to the armory!");
        Console.WriteLine($"You've got {fighter.Gold}g to spend. Choose a weapon to take into the fight:");

        for (var i = 0; i < _weaponOffers.Count; i++)
        {
            var (weapon, cost) = _weaponOffers[i];
            Console.WriteLine($"{i + 1}. {weapon.Name} (Damage: {weapon.Damage}) - {cost}g");
        }

        while (true)
        {
            Console.Write("Enter weapon number to purchase or press Enter to keep your current weapon: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"You keep your {fighter.Weapon.Name} and {fighter.Gold}g.");
                break;
            }

            if (!int.TryParse(input, out var choice) || choice < 1 || choice > _weaponOffers.Count)
            {
                Console.WriteLine("Invalid selection. Choose a number from the list.");
                continue;
            }

            var (selectedWeapon, cost) = _weaponOffers[choice - 1];

            if (!fighter.TrySpendGold(cost))
            {
                Console.WriteLine("Not enough gold. Pick something cheaper.");
                continue;
            }

            fighter.EquipWeapon(selectedWeapon);
            Console.WriteLine($"You purchased the {selectedWeapon.Name}! Remaining gold: {fighter.Gold}g.");
            break;
        }
    }
}
