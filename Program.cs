using System;
using System.Collections.Generic;
using fights;

var weaponOffers = new List<(Weapon weapon, uint cost)>
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
    gold: 100u);

Console.WriteLine("Welcome to the armory! You've got 100g to spend.");
Console.WriteLine("Choose a weapon to take into the fight:");

for (var i = 0; i < weaponOffers.Count; i++)
{
    var (weapon, cost) = weaponOffers[i];
    Console.WriteLine($"{i + 1}. {weapon.Name} (Damage: {weapon.Damage}) - {cost}g");
}

while (true)
{
    Console.Write("Enter weapon number to purchase or press Enter to keep your current weapon: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        Console.WriteLine($"You keep your {jackson.Weapon.Name} and {jackson.Gold}g.");
        break;
    }

    if (!int.TryParse(input, out var choice) || choice < 1 || choice > weaponOffers.Count)
    {
        Console.WriteLine("Invalid selection. Choose a number from the list.");
        continue;
    }

    var (selectedWeapon, cost) = weaponOffers[choice - 1];

    if (!jackson.TrySpendGold(cost))
    {
        Console.WriteLine("Not enough gold. Pick something cheaper.");
        continue;
    }

    jackson.EquipWeapon(selectedWeapon);
    Console.WriteLine($"You purchased the {selectedWeapon.Name}! Remaining gold: {jackson.Gold}g.");
    break;
}

var enemies = new List<Fighter>
{
    new Fighter(name: "Nerd", health: 120, weapon: new Weapon(name: "Stick", damage: 1), gold: 0u),
    new Fighter(name: "Goblin", health: 80, weapon: new Weapon(name: "Rusty Blade", damage: 4), gold: 0u),
    new Fighter(name: "Orc Warrior", health: 140, weapon: new Weapon(name: "Heavy Club", damage: 7), gold: 0u),
    new Fighter(name: "Skeleton Knight", health: 110, weapon: new Weapon(name: "Ancient Sword", damage: 6), gold: 0u)
};

var random = new Random();
var enemy = enemies[random.Next(enemies.Count)];

Console.WriteLine($"A wild {enemy.Name} appears! Prepare for battle.");

var fight = new Fight(jackson, enemy);
fight.Start();
