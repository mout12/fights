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

var armorOffers = new List<(IArmor armor, uint cost)>
{
    (new Armor(name: "Padded Vest", defense: 2), 35u),
    (new Armor(name: "Chain Shirt", defense: 4), 60u),
    (new Armor(name: "Enchanted Robes", defense: 5), 75u),
    (new Armor(name: "Plate Mail", defense: 6), 90u)
};

var jackson = new Fighter(
    name: "Jackson",
    health: 100,
    weapon: new Weapon(name: "Fists", damage: 1),
    armor: new Armor(name: "Cloth Shirt", defense: 1),
    gold: 100u);

var blacksmith = new Blacksmith(weaponOffers);
var armorer = new Armorer(armorOffers);

var enemies = new List<Fighter>
{
    new Fighter(name: "Nerd", health: 120, weapon: new Weapon(name: "Stick", damage: 1), armor: new Armor(name: "Thick Glasses", defense: 1), gold: 5u),
    new Fighter(name: "Goblin", health: 80, weapon: new Weapon(name: "Rusty Blade", damage: 4), armor: new Armor(name: "Leather Scraps", defense: 2), gold: 12u),
    new Fighter(name: "Orc Warrior", health: 140, weapon: new Weapon(name: "Heavy Club", damage: 7), armor: new Armor(name: "Chain Vest", defense: 3), gold: 20u),
    new Fighter(name: "Skeleton Knight", health: 110, weapon: new Weapon(name: "Ancient Sword", damage: 6), armor: new Armor(name: "Bone Plating", defense: 2), gold: 15u)
};

var town = new Town(jackson, blacksmith, armorer, enemies);
town.Enter();
