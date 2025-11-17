using System;
using System.Collections.Generic;
using fights;

var weaponOffers = new List<(IWeapon weapon, uint cost)>
{
    (new Weapon(name: "Dagger", damage: 5), 30u),
    (new Weapon(name: "Short Sword", damage: 7), 55u),
    (new Weapon(name: "War Axe", damage: 9), 80u),
    (new Weapon(name: "Wizard Staff", damage: 11), 100u)
};

var jackson = new Fighter(
    name: "Jackson",
    health: 100,
    weapon: new Weapon(name: "Fists", damage: 1),
    armor: new Armor(name: "Cloth Shirt", defense: 1),
    gold: 100u);

var blacksmith = new Blacksmith(weaponOffers);
blacksmith.Enter(jackson);

var enemies = new List<Fighter>
{
    new Fighter(name: "Nerd", health: 120, weapon: new Weapon(name: "Stick", damage: 1), armor: new Armor(name: "Thick Glasses", defense: 1), gold: 0u),
    new Fighter(name: "Goblin", health: 80, weapon: new Weapon(name: "Rusty Blade", damage: 4), armor: new Armor(name: "Leather Scraps", defense: 2), gold: 0u),
    new Fighter(name: "Orc Warrior", health: 140, weapon: new Weapon(name: "Heavy Club", damage: 7), armor: new Armor(name: "Chain Vest", defense: 3), gold: 0u),
    new Fighter(name: "Skeleton Knight", health: 110, weapon: new Weapon(name: "Ancient Sword", damage: 6), armor: new Armor(name: "Bone Plating", defense: 2), gold: 0u)
};

var random = new Random();
var enemy = enemies[random.Next(enemies.Count)];

Console.WriteLine($"A wild {enemy.Name} appears! Prepare for battle.");

var fight = new Fight(jackson, enemy);
fight.Start();
