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
        if (ResolvePoisonDamage(attacker, defender))
        {
            return true;
        }

        var weapon = attacker.Weapon;
        var payload = weapon.CreateDamagePayload();
        var damageDealt = defender.TakeDamage(payload);
        var criticalSuffix = payload.IsCritical ? " (critical strike!)" : string.Empty;
        var selfDamageSuffix = ApplySelfDamage(attacker, payload);
        var verb = isCounterAttack ? "retaliates" : "attacks";
        Console.WriteLine($"{attacker.Name} {verb} with {weapon.Name} for {damageDealt} damage{criticalSuffix}{selfDamageSuffix}. {defender.Name} has {defender.Health} health remaining.");
        TryInflictPoison(defender, payload.PoisonToApply);

        if (defender.Health <= 0)
        {
            return HandleVictory(attacker, defender, $"{defender.Name} has been defeated!");
        }

        if (attacker.Health <= 0)
        {
            var selfDeathMessage = isCounterAttack
                ? $"{attacker.Name}'s wild swing backfires!"
                : $"{attacker.Name}'s reckless attack proves fatal!";
            return HandleVictory(defender, attacker, selfDeathMessage);
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

    private static bool ResolvePoisonDamage(IFighter fighter, IFighter opponent)
    {
        var poisonTick = fighter.TickPoison();
        if (!poisonTick.HadPoison)
        {
            return false;
        }

        if (poisonTick.Damage > 0)
        {
            fighter.TakeSelfDamage(poisonTick.Damage);
            var remainingSuffix = poisonTick.RemainingTurns > 0
                ? $" ({poisonTick.RemainingTurns} turns of poison remain)"
                : string.Empty;
            Console.WriteLine($"{fighter.Name} suffers {poisonTick.Damage} poison damage{remainingSuffix}.");

            if (fighter.Health <= 0)
            {
                return HandleVictory(opponent, fighter, $"{fighter.Name} succumbs to the poison!");
            }
        }

        if (poisonTick.RemainingTurns <= 0)
        {
            Console.WriteLine($"{fighter.Name} is no longer poisoned.");
        }

        return false;
    }

    private static void TryInflictPoison(IFighter defender, PoisonState? poison)
    {
        if (poison is null || !poison.Value.HasEffect)
        {
            return;
        }

        var previousState = defender.ActivePoison;
        defender.ApplyPoison(poison.Value);
        var currentState = defender.ActivePoison;
        if (currentState is null)
        {
            return;
        }

        var message = previousState is null
            ? $"{defender.Name} is poisoned!"
            : $"{defender.Name}'s poison worsens!";

        var snapshot = currentState.Value;
        Console.WriteLine($"{message} {snapshot.DamagePerTurn} damage for {snapshot.RemainingTurns} turns ({snapshot.TickChancePercent}% chance each turn).");
    }

    private static bool HandleVictory(IFighter winner, IFighter loser, string defeatMessage)
    {
        Console.WriteLine(defeatMessage);
        if (loser.Gold > 0)
        {
            winner.GainGold(loser.Gold);
            Console.WriteLine($"{winner.Name} loots {loser.Gold} gold.");
        }
        Console.WriteLine($"{winner.Name} wins!");
        return true;
    }
}
