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
        Console.WriteLine($"Enter 'a' to attack or 'r' to run.");

        while (true)
        {
            Console.Write("> ");
            var choice = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (choice == "r")
            {
                Console.WriteLine($"{_fighterOne.Name} decides to live another day.");
                break;
            }

            if (choice != "a")
            {
                Console.WriteLine("Invalid choice. Enter 'a' to attack or 'r' to run.");
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
        _fighterTwo.TakeDamage(_fighterOne.Damage);
        Console.WriteLine($"{_fighterOne.Name} strikes for {_fighterOne.Damage} damage. {_fighterTwo.Name} has {_fighterTwo.Health} health remaining.");

        if (_fighterTwo.Health <= 0)
        {
            Console.WriteLine($"{_fighterTwo.Name} has been defeated!");
            Console.WriteLine($"{_fighterOne.Name} wins!");
            return true;
        }

        _fighterOne.TakeDamage(_fighterTwo.Damage);
        Console.WriteLine($"{_fighterTwo.Name} retaliates for {_fighterTwo.Damage} damage. {_fighterOne.Name} has {_fighterOne.Health} health remaining.");

        if (_fighterOne.Health <= 0)
        {
            Console.WriteLine($"{_fighterOne.Name} has been defeated!");
            Console.WriteLine($"{_fighterTwo.Name} wins!");
            return true;
        }

        return false;
    }
}
