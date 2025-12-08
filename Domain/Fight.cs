using System;
using System.Linq;

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
        var playerStrikes = GetStrikeCount(_fighterOne.Weapon);
        for (var i = 0; i < playerStrikes; i++)
        {
            if (PerformStrike(_fighterOne, _fighterTwo, isCounterAttack: false))
            {
                return true;
            }
        }

        var enemyStrikes = GetStrikeCount(_fighterTwo.Weapon);
        for (var i = 0; i < enemyStrikes; i++)
        {
            if (PerformStrike(_fighterTwo, _fighterOne, isCounterAttack: true))
            {
                return true;
            }
        }

        return false;
    }

    private static bool PerformStrike(IFighter attacker, IFighter defender, bool isCounterAttack)
    {
        var weapon = attacker.Weapon;
        var payload = weapon.CreateDamagePayload();
        var damageDealt = defender.TakeDamage(payload);
        var criticalSuffix = payload.IsCritical ? " (critical strike!)" : string.Empty;
        var selfDamageSuffix = ApplySelfDamage(attacker, payload);
        var verb = isCounterAttack ? "retaliates" : "attacks";
        Console.WriteLine($"{attacker.Name} {verb} with {weapon.Name} for {damageDealt} damage{criticalSuffix}{selfDamageSuffix}. {defender.Name} has {defender.Health} health remaining.");

        if (defender.Health <= 0)
        {
            Console.WriteLine($"{defender.Name} has been defeated!");
            if (defender.Gold > 0)
            {
                attacker.GainGold(defender.Gold);
                Console.WriteLine($"{attacker.Name} loots {defender.Gold} gold.");
            }
            Console.WriteLine($"{attacker.Name} wins!");
            return true;
        }

        if (attacker.Health <= 0)
        {
            var selfDeathMessage = isCounterAttack
                ? $"{attacker.Name}'s wild swing backfires!"
                : $"{attacker.Name}'s reckless attack proves fatal!";
            Console.WriteLine(selfDeathMessage);
            if (attacker.Gold > 0)
            {
                defender.GainGold(attacker.Gold);
                Console.WriteLine($"{defender.Name} loots {attacker.Gold} gold.");
            }
            Console.WriteLine($"{defender.Name} wins!");
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

    private static int GetStrikeCount(IWeapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        return weapon.Modifiers.OfType<DoubleStrikeWeaponModifier>().Any() ? 2 : 1;
    }
}
