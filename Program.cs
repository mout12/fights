using System;
using System.Collections.Generic;
using fights;

Player CreateNewPlayer()
{
    return new Player(
        name: "Jackson",
        level: 1,
        health: 100,
        weapon: new Weapon(name: "Fists", damage: 1),
        armor: new Armor(name: "Cloth Shirt", defense: 1),
        gold: 1000u);
}

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

var blacksmith = new Blacksmith(weaponOffers);
var armorer = new Armorer(armorOffers);
var healersHut = new HealersHut();
var saveGameService = new SaveGameService();

var levelOneEnemies = new List<Fighter>
{
    new Fighter(name: "Nerd", health: 120, weapon: new Weapon(name: "Stick", damage: 1), armor: new Armor(name: "Thick Glasses", defense: 1), gold: 5u),
    new Fighter(name: "Goblin", health: 80, weapon: new Weapon(name: "Rusty Blade", damage: 4), armor: new Armor(name: "Leather Scraps", defense: 2), gold: 12u),
    new Fighter(name: "Rabid Dog", health: 70, weapon: new Weapon(name: "Fangs", damage: 5), armor: new Armor(name: "Matted Fur", defense: 1), gold: 8u),
    new Fighter(name: "Bandit", health: 90, weapon: new Weapon(name: "Shiv", damage: 6), armor: new Armor(name: "Patchwork Vest", defense: 2), gold: 15u)
};

var levelTwoEnemies = new List<Fighter>
{
    new Fighter(name: "Orc Warrior", health: 140, weapon: new Weapon(name: "Heavy Club", damage: 7), armor: new Armor(name: "Chain Vest", defense: 3), gold: 20u),
    new Fighter(name: "Skeleton Knight", health: 110, weapon: new Weapon(name: "Ancient Sword", damage: 6), armor: new Armor(name: "Bone Plating", defense: 2), gold: 15u),
    new Fighter(name: "Dark Archer", health: 100, weapon: new Weapon(name: "Shadow Bow", damage: 8), armor: new Armor(name: "Hooded Cloak", defense: 3), gold: 18u),
    new Fighter(name: "Troll Bruiser", health: 160, weapon: new Weapon(name: "Stone Hammer", damage: 9), armor: new Armor(name: "Thick Hide", defense: 4), gold: 25u)
};

var levelContents = new Dictionary<int, LevelContent>
{
    [1] = new LevelContent(levelOneEnemies, new Boss(name: "Dragon Whelp", level: 1, health: 200, weapon: new Weapon(name: "Flame Breath", damage: 12), armor: new Armor(name: "Scale Hide", defense: 4), gold: 50u)),
    [2] = new LevelContent(levelTwoEnemies, new Boss(name: "Lich Lord", level: 2, health: 260, weapon: new Weapon(name: "Soul Drain", damage: 15), armor: new Armor(name: "Shadow Shroud", defense: 5), gold: 75u))
};

var player = InitializePlayer(saveGameService);
var town = new Town(player, blacksmith, armorer, healersHut, levelContents);
var leftTownPeacefully = town.Enter();

if (leftTownPeacefully)
{
    saveGameService.Save(player);
    Console.WriteLine("Your progress has been saved.");
}
else
{
    Console.WriteLine("Progress was not saved.");
}

Player InitializePlayer(SaveGameService saveService)
{
    if (!saveService.HasSave())
    {
        return CreateNewPlayer();
    }

    Console.WriteLine("A previous adventure was found. Choose an option:");
    Console.WriteLine("1. Start over");
    Console.WriteLine("2. Load previous adventure");
    Console.Write("> ");

    var choice = Console.ReadLine()?.Trim();
    if (choice == "2")
    {
        var loadedPlayer = saveService.TryLoad();
        if (loadedPlayer is not null)
        {
            Console.WriteLine("Previous adventure loaded.");
            return loadedPlayer;
        }

        Console.WriteLine("Failed to load the previous adventure. Starting fresh.");
    }
    else
    {
        Console.WriteLine("Starting a new adventure.");
    }

    return CreateNewPlayer();
}
