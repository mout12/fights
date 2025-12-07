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

    public Town(Player player, Blacksmith blacksmith, Armorer armorer, HealersHut healersHut, Dictionary<int, LevelContent> levels)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _blacksmith = blacksmith ?? throw new ArgumentNullException(nameof(blacksmith));
        _armorer = armorer ?? throw new ArgumentNullException(nameof(armorer));
        _healersHut = healersHut ?? throw new ArgumentNullException(nameof(healersHut));
        _levels = levels ?? throw new ArgumentNullException(nameof(levels));

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
            Console.WriteLine("1. Visit the blacksmith");
            Console.WriteLine("2. Visit the armorer");
            Console.WriteLine("3. Visit the healer's hut");
            Console.WriteLine("4. Venture out and fight");
            var currentBoss = _levels.TryGetValue(_player.Level, out var level) ? level.Boss : null;
            var bossOption = currentBoss is null
                ? "5. Challenge the boss (none available for your level)"
                : $"5. Challenge the boss: {currentBoss.Name} (Level {currentBoss.Level})";
            Console.WriteLine(bossOption);
            Console.WriteLine("6. Leave town");
            Console.Write("> ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    _blacksmith.Enter(_player);
                    break;
                case "2":
                    _armorer.Enter(_player);
                    break;
                case "3":
                    _healersHut.Enter(_player);
                    break;
                case "4":
                    if (!StartFight())
                    {
                        Console.WriteLine("Your adventure ends here.");
                        return false;
                    }
                    break;
                case "5":
                    if (!StartBossFight())
                    {
                        Console.WriteLine("Your adventure ends here.");
                        return false;
                    }
                    break;
                case "6":
                    Console.WriteLine("You decide to rest and leave the adventure for another day.");
                    return true;
                default:
                    Console.WriteLine("Invalid choice. Please select 1, 2, 3, 4, 5, or 6.");
                    break;
            }
        }
    }

    private bool StartFight()
    {
        if (!_levels.TryGetValue(_player.Level, out var level) || level.Enemies.Count == 0)
        {
            Console.WriteLine("No enemies are available for your current level. Try a different challenge.");
            return true;
        }

        var enemy = level.Enemies[Random.Shared.Next(level.Enemies.Count)];
        Console.WriteLine($"A wild {enemy.Name} appears! Prepare for battle.");

        var fight = new Fight(_player, enemy);
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
        var fight = new Fight(_player, boss);
        var playerSurvived = fight.Start();

        if (playerSurvived && boss.Health <= 0)
        {
            _player.LevelUp();
            Console.WriteLine($"You feel stronger! You are now level {_player.Level}.");
        }

        return playerSurvived;
    }
}
