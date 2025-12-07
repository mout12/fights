using System;

namespace fights;

public class Fight
{
    private readonly IFighter _fighterOne;
    private readonly IFighter _fighterTwo;
    private readonly IInputSelectionService _inputSelector;

    public Fight(IFighter fighterOne, IFighter fighterTwo, IInputSelectionService inputSelector)
    {
        _fighterOne = fighterOne;
        _fighterTwo = fighterTwo;
        _inputSelector = inputSelector ?? throw new ArgumentNullException(nameof(inputSelector));
    }

    public bool Start()
    {
        Console.WriteLine($"A fight begins between {_fighterOne.Name} and {_fighterTwo.Name}!");
        Console.WriteLine("Press the highlighted key for your action.");

        while (true)
        {
            var action = _inputSelector.SelectOption(
                "Choose your action:",
                new[]
                {
                    new InputOption<Func<bool>>("[A]ttack", () =>
                    {
                        var finished = ExecuteTurn();
                        return finished;
                    }, Hotkey: 'a'),
                    new InputOption<Func<bool>>("[R]un Away", () =>
                    {
                        Console.WriteLine($"{_fighterOne.Name} decides to live another day.");
                        PauseBeforeExit();
                        return true;
                    }, Hotkey: 'r')
                });

            var actionResult = action();
            if (actionResult)
            {
                if (_fighterTwo.Health <= 0 || _fighterOne.Health <= 0)
                {
                    PauseBeforeExit();
                }
                return _fighterOne.Health > 0;
            }

            if (_fighterOne.Health <= 0)
            {
                PauseBeforeExit();
                return false;
            }
        }
    }

    private bool ExecuteTurn()
    {
        var firstStrikeWeapon = _fighterOne.Weapon;
        var firstStrikePayload = firstStrikeWeapon.CreateDamagePayload();
        var firstStrikeDealt = _fighterTwo.TakeDamage(firstStrikePayload);
        var firstStrikeSuffix = firstStrikePayload.IsCritical ? " (critical strike!)" : string.Empty;
        var selfDamageSuffix = ApplySelfDamage(_fighterOne, firstStrikePayload);
        Console.WriteLine($"{_fighterOne.Name} attacks with {firstStrikeWeapon.Name} for {firstStrikeDealt} damage{firstStrikeSuffix}{selfDamageSuffix}. {_fighterTwo.Name} has {_fighterTwo.Health} health remaining.");

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

        if (_fighterOne.Health <= 0)
        {
            Console.WriteLine($"{_fighterOne.Name}'s reckless attack proves fatal!");
            if (_fighterOne.Gold > 0)
            {
                _fighterTwo.GainGold(_fighterOne.Gold);
                Console.WriteLine($"{_fighterTwo.Name} loots {_fighterOne.Gold} gold.");
            }
            Console.WriteLine($"{_fighterTwo.Name} wins!");
            return true;
        }

        var counterStrikeWeapon = _fighterTwo.Weapon;
        var counterStrikePayload = counterStrikeWeapon.CreateDamagePayload();
        var counterStrikeDealt = _fighterOne.TakeDamage(counterStrikePayload);
        var counterStrikeSuffix = counterStrikePayload.IsCritical ? " (critical strike!)" : string.Empty;
        var counterSelfDamage = ApplySelfDamage(_fighterTwo, counterStrikePayload);
        Console.WriteLine($"{_fighterTwo.Name} retaliates with {counterStrikeWeapon.Name} for {counterStrikeDealt} damage{counterStrikeSuffix}{counterSelfDamage}. {_fighterOne.Name} has {_fighterOne.Health} health remaining.");

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

        if (_fighterTwo.Health <= 0)
        {
            Console.WriteLine($"{_fighterTwo.Name}'s wild swing backfires!");
            if (_fighterTwo.Gold > 0)
            {
                _fighterOne.GainGold(_fighterTwo.Gold);
                Console.WriteLine($"{_fighterOne.Name} loots {_fighterTwo.Gold} gold.");
            }
            Console.WriteLine($"{_fighterOne.Name} wins!");
            return true;
        }

        return false;
    }

    private static string ApplySelfDamage(IFighter attacker, IDamagePayload payload)
    {
        if (payload.SelfDamage <= 0)
        {
            return string.Empty;
        }

        attacker.TakeSelfDamage(payload.SelfDamage);
        return $", but takes {payload.SelfDamage} self-damage";
    }

    private static void PauseBeforeExit()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(intercept: true);
        Console.WriteLine();
    }
}
