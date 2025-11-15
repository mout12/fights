using System;
using System.Collections.Generic;
using fights;

var jackson = new Fighter(
    name: "Jackson",
    health: 100,
    weapon: new Weapon(name: "Dagger", damage: 5));

var enemies = new List<Fighter>
{
    new Fighter(name: "Nerd", health: 120, weapon: new Weapon(name: "Stick", damage: 1)),
    new Fighter(name: "Goblin", health: 80, weapon: new Weapon(name: "Rusty Blade", damage: 4)),
    new Fighter(name: "Orc Warrior", health: 140, weapon: new Weapon(name: "Heavy Club", damage: 7)),
    new Fighter(name: "Skeleton Knight", health: 110, weapon: new Weapon(name: "Ancient Sword", damage: 6))
};

var random = new Random();
var enemy = enemies[random.Next(enemies.Count)];

var fight = new Fight(jackson, enemy);
fight.Start();
