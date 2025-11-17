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

    public void Start()
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
                break;
            }

            if (choice != 'a')
            {
                Console.WriteLine("Invalid choice. Press 'a' to attack or 'r' to run.");
                continue;
            }

            if (ExecuteTurn())
            {
                break;
            }
        }
    }

    private bool ExecuteTurn()
    {
        var firstStrikeWeapon = _fighterOne.Weapon;
        var firstStrikeDamage = firstStrikeWeapon.Damage;
        var firstStrikeDealt = _fighterTwo.TakeDamage(firstStrikeWeapon);
        Console.WriteLine($"{_fighterOne.Name} attacks with {firstStrikeWeapon.Name} for {firstStrikeDealt} damage. {_fighterTwo.Name} has {_fighterTwo.Health} health remaining.");

        if (_fighterTwo.Health <= 0)
        {
            Console.WriteLine($"{_fighterTwo.Name} has been defeated!");
            Console.WriteLine($"{_fighterOne.Name} wins!");
            return true;
        }

        var counterStrikeWeapon = _fighterTwo.Weapon;
        var counterStrikeDamage = counterStrikeWeapon.Damage;
        var counterStrikeDealt = _fighterOne.TakeDamage(counterStrikeWeapon);
        Console.WriteLine($"{_fighterTwo.Name} retaliates with {counterStrikeWeapon.Name} for {counterStrikeDealt} damage. {_fighterOne.Name} has {_fighterOne.Health} health remaining.");

        if (_fighterOne.Health <= 0)
        {
            Console.WriteLine($"{_fighterOne.Name} has been defeated!");
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
