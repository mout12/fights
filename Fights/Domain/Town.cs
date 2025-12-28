using System;
using System.Collections.Generic;

namespace fights;

public class Town
{
    private readonly Player _player;
    private readonly Blacksmith _blacksmith;
    private readonly Armorer _armorer;
    private readonly HealersHut _healersHut;
    private readonly Dictionary<int, LevelContent> _levels;
    private readonly IInputSelectionService _inputSelector;

    public Town(
        Player player,
        Blacksmith blacksmith,
        Armorer armorer,
        HealersHut healersHut,
        Dictionary<int, LevelContent> levels,
        IInputSelectionService inputSelector)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _blacksmith = blacksmith ?? throw new ArgumentNullException(nameof(blacksmith));
        _armorer = armorer ?? throw new ArgumentNullException(nameof(armorer));
        _healersHut = healersHut ?? throw new ArgumentNullException(nameof(healersHut));
        _levels = levels ?? throw new ArgumentNullException(nameof(levels));
        _inputSelector = inputSelector ?? throw new ArgumentNullException(nameof(inputSelector));

        if (_levels.Count == 0)
        {
            throw new ArgumentException("Town must define at least one level of content.", nameof(levels));
        }
    }

    public bool Enter()
    {
        Console.WriteLine("You arrive at the town square. Where will you go?");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"Status: Level={_player.Level} | Weapon={_player.Weapon.Name} | Armor={_player.Armor.Name} | Gold={_player.Gold}g | Health={_player.Health}/{_player.MaxHealth}");
            var currentBoss = _levels.TryGetValue(_player.Level, out var level) ? level.Boss : null;
            var bossDescription = currentBoss is null
                ? "[C]hallenge the boss (none available for your level)"
                : $"[C]hallenge the boss: {currentBoss.Name} (Level {currentBoss.Level})";

            var options = new[]
            {
                new InputOption<Func<TownLoopResult>>("Visit the [B]lacksmith", () => { _blacksmith.Enter(_player); return TownLoopResult.Continue; }, Hotkey: 'b'),
                new InputOption<Func<TownLoopResult>>("Visit the [A]rmorer", () => { _armorer.Enter(_player); return TownLoopResult.Continue; }, Hotkey: 'a'),
                new InputOption<Func<TownLoopResult>>("Visit the [H]ealer's hut", () => { _healersHut.Enter(_player); return TownLoopResult.Continue; }, Hotkey: 'h'),
                new InputOption<Func<TownLoopResult>>("Venture out and [F]ight", () => StartFight() ? TownLoopResult.Continue : TownLoopResult.AdventureEnded, Hotkey: 'f'),
                new InputOption<Func<TownLoopResult>>(bossDescription, () => StartBossFight() ? TownLoopResult.Continue : TownLoopResult.AdventureEnded, Hotkey: 'c'),
                new InputOption<Func<TownLoopResult>>("[Q]uit to fields", () => ConfirmQuit(), Hotkey: 'q')
            };

            var action = _inputSelector.SelectOption("Where will you go?", options);
            var result = action();
            if (result == TownLoopResult.LeftTown)
            {
                return true;
            }

            if (result == TownLoopResult.AdventureEnded)
            {
                Console.WriteLine("Your adventure ends here.");
                return false;
            }
        }
    }

    private TownLoopResult ConfirmQuit()
    {
        var selection = _inputSelector.SelectOption(
            "Are you sure you want to leave town?",
            new[]
            {
                new InputOption<Func<TownLoopResult>>("[Y]es", () =>
                {
                    Console.WriteLine("You decide to rest and leave the adventure for another day.");
                    return TownLoopResult.LeftTown;
                }, Hotkey: 'y'),
                new InputOption<Func<TownLoopResult>>("[N]o", () =>
                {
                    Console.WriteLine("You stay in town, determined to continue the adventure.");
                    return TownLoopResult.Continue;
                }, Hotkey: 'n')
            });

        return selection();
    }

    private bool StartFight()
    {
        if (!_levels.TryGetValue(_player.Level, out var level) || level.Enemies.Count == 0)
        {
            Console.WriteLine("No enemies are available for your current level. Try a different challenge.");
            return true;
        }

        var enemy = level.Enemies[GameRandom.Current.Next(level.Enemies.Count)];
        Console.WriteLine($"A wild {enemy.Name} appears! Prepare for battle.");

        var fight = new Fight(_player, enemy, _inputSelector);
        return fight.Start();
    }

    private bool StartBossFight()
    {
        if (!_levels.TryGetValue(_player.Level, out var level))
        {
            Console.WriteLine("No boss is available for your current level.");
            return true;
        }

        var boss = level.Boss;
        if (boss == null)
        {
            Console.WriteLine("No boss is available for your current level.");
            return true;
        }

        Console.WriteLine($"You challenge the boss of level {_player.Level}: {boss.Name}!");
        var fight = new Fight(_player, boss, _inputSelector);
        var playerSurvived = fight.Start();

        if (playerSurvived && boss.Health <= 0)
        {
            _player.LevelUp();
            Console.WriteLine($"You feel stronger! You are now level {_player.Level}.");
        }

        return playerSurvived;
    }

    private enum TownLoopResult
    {
        Continue,
        AdventureEnded,
        LeftTown
    }
}
