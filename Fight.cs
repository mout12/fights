using System;

namespace fights;

public class Fight
{
    private readonly IFighter _fighterOne;
    private readonly IFighter _fighterTwo;

    public Fight(IFighter fighterOne, IFighter fighterTwo)
    {
        _fighterOne = fighterOne;
        _fighterTwo = fighterTwo;
    }

    public bool Start()
    {
        Console.WriteLine($"A fight begins between {_fighterOne.Name} and {_fighterTwo.Name}!");
        Console.WriteLine($"Press 'a' to attack or 'r' to run.");

        while (true)
        {
            Console.Write("> ");
            var choice = ReadChoice();

            if (choice == 'r')
            {
                Console.WriteLine($"{_fighterOne.Name} decides to live another day.");
                return true;
            }

            if (choice != 'a')
            {
                Console.WriteLine("Invalid choice. Press 'a' to attack or 'r' to run.");
                continue;
            }

            if (ExecuteTurn())
            {
                return _fighterOne.Health > 0;
            }
        }
    }

    private bool ExecuteTurn()
    {
        var firstStrikeWeapon = _fighterOne.Weapon;
        var firstStrikePayload = firstStrikeWeapon.CreateDamagePayload();
        var firstStrikeDealt = _fighterTwo.TakeDamage(firstStrikePayload);
        var firstStrikeSuffix = firstStrikePayload.IsCritical ? " (critical strike!)" : string.Empty;
        Console.WriteLine($"{_fighterOne.Name} attacks with {firstStrikeWeapon.Name} for {firstStrikeDealt} damage{firstStrikeSuffix}. {_fighterTwo.Name} has {_fighterTwo.Health} health remaining.");

        if (_fighterTwo.Health <= 0)
        {
            Console.WriteLine($"{_fighterTwo.Name} has been defeated!");
            if (_fighterTwo.Gold > 0)
            {
                _fighterOne.GainGold(_fighterTwo.Gold);
                Console.WriteLine($"{_fighterOne.Name} loots {_fighterTwo.Gold} gold.");
            }
            Console.WriteLine($"{_fighterOne.Name} wins!");
            return true;
        }

        var counterStrikeWeapon = _fighterTwo.Weapon;
        var counterStrikePayload = counterStrikeWeapon.CreateDamagePayload();
        var counterStrikeDealt = _fighterOne.TakeDamage(counterStrikePayload);
        var counterStrikeSuffix = counterStrikePayload.IsCritical ? " (critical strike!)" : string.Empty;
        Console.WriteLine($"{_fighterTwo.Name} retaliates with {counterStrikeWeapon.Name} for {counterStrikeDealt} damage{counterStrikeSuffix}. {_fighterOne.Name} has {_fighterOne.Health} health remaining.");

        if (_fighterOne.Health <= 0)
        {
            Console.WriteLine($"{_fighterOne.Name} has been defeated!");
            if (_fighterOne.Gold > 0)
            {
                _fighterTwo.GainGold(_fighterOne.Gold);
                Console.WriteLine($"{_fighterTwo.Name} loots {_fighterOne.Gold} gold.");
            }
            Console.WriteLine($"{_fighterTwo.Name} wins!");
            return true;
        }

        return false;
    }

    private char ReadChoice()
    {
        try
        {
            if (!Console.IsInputRedirected)
            {
                var key = Console.ReadKey(intercept: true);
                Console.WriteLine(); // move to next line after key press
                return char.ToLowerInvariant(key.KeyChar);
            }
        }
        catch (InvalidOperationException)
        {
            // fall through to ReadLine below if ReadKey isn't available
        }

        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
        {
            return '\0';
        }

        return char.ToLowerInvariant(line.Trim()[0]);
    }
}
