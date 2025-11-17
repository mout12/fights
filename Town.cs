using System;
using System.Collections.Generic;

namespace fights;

public class Town
{
    private readonly Player _player;
    private readonly Blacksmith _blacksmith;
    private readonly Armorer _armorer;
    private readonly List<Fighter> _enemies;
    private readonly List<Boss> _bosses;

    public Town(Player player, Blacksmith blacksmith, Armorer armorer, List<Fighter> enemies, List<Boss> bosses)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _blacksmith = blacksmith ?? throw new ArgumentNullException(nameof(blacksmith));
        _armorer = armorer ?? throw new ArgumentNullException(nameof(armorer));
        _enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));
        _bosses = bosses ?? throw new ArgumentNullException(nameof(bosses));

        if (_enemies.Count == 0)
        {
            throw new ArgumentException("Town must have at least one enemy to fight.", nameof(enemies));
        }

        if (_bosses.Count == 0)
        {
            throw new ArgumentException("Town must have at least one boss to fight.", nameof(bosses));
        }
    }

    public void Enter()
    {
        Console.WriteLine("You arrive at the town square. Where will you go?");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"Status: Level={_player.Level} | Weapon={_player.Weapon.Name} | Armor={_player.Armor.Name} | Gold={_player.Gold}g | Health={_player.Health}");
            Console.WriteLine("1. Visit the blacksmith");
            Console.WriteLine("2. Visit the armorer");
            Console.WriteLine("3. Venture out and fight");
            var currentBoss = _bosses.Find(b => b.Level == _player.Level);
            var bossOption = currentBoss is null
                ? "4. Challenge the boss (none available for your level)"
                : $"4. Challenge the boss: {currentBoss.Name} (Level {currentBoss.Level})";
            Console.WriteLine(bossOption);
            Console.WriteLine("5. Leave town");
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
                    if (!StartFight())
                    {
                        Console.WriteLine("Your adventure ends here.");
                        return;
                    }
                    break;
                case "4":
                    if (!StartBossFight())
                    {
                        Console.WriteLine("Your adventure ends here.");
                        return;
                    }
                    break;
                case "5":
                    Console.WriteLine("You decide to rest and leave the adventure for another day.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select 1, 2, 3, 4, or 5.");
                    break;
            }
        }
    }

    private bool StartFight()
    {
        var enemy = _enemies[Random.Shared.Next(_enemies.Count)];
        Console.WriteLine($"A wild {enemy.Name} appears! Prepare for battle.");

        var fight = new Fight(_player, enemy);
        return fight.Start();
    }

    private bool StartBossFight()
    {
        var boss = _bosses.Find(b => b.Level == _player.Level);
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
