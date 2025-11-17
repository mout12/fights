using System;
using System.Collections.Generic;

namespace fights;

public class Town
{
    private readonly Fighter _player;
    private readonly Blacksmith _blacksmith;
    private readonly Armorer _armorer;
    private readonly List<Fighter> _enemies;

    public Town(Fighter player, Blacksmith blacksmith, Armorer armorer, List<Fighter> enemies)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _blacksmith = blacksmith ?? throw new ArgumentNullException(nameof(blacksmith));
        _armorer = armorer ?? throw new ArgumentNullException(nameof(armorer));
        _enemies = enemies ?? throw new ArgumentNullException(nameof(enemies));

        if (_enemies.Count == 0)
        {
            throw new ArgumentException("Town must have at least one enemy to fight.", nameof(enemies));
        }
    }

    public void Enter()
    {
        Console.WriteLine("You arrive at the town square. Where will you go?");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"Status: Weapon={_player.Weapon.Name} | Armor={_player.Armor.Name} | Gold={_player.Gold}g | Health={_player.Health}");
            Console.WriteLine("1. Visit the blacksmith");
            Console.WriteLine("2. Visit the armorer");
            Console.WriteLine("3. Venture out and fight");
            Console.WriteLine("4. Leave town");
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
                    StartFight();
                    return;
                case "4":
                    Console.WriteLine("You decide to rest and leave the adventure for another day.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please select 1, 2, 3, or 4.");
                    break;
            }
        }
    }

    private void StartFight()
    {
        var enemy = _enemies[Random.Shared.Next(_enemies.Count)];
        Console.WriteLine($"A wild {enemy.Name} appears! Prepare for battle.");

        var fight = new Fight(_player, enemy);
        fight.Start();
    }
}
