using System;
using System.Collections.Generic;
using fights;

var weaponCatalog = new Dictionary<string, IWeapon>(StringComparer.OrdinalIgnoreCase);
var armorCatalog = new Dictionary<string, IArmor>(StringComparer.OrdinalIgnoreCase);

IWeapon RegisterWeapon(IWeapon weapon)
{
    weaponCatalog[weapon.Name] = weapon;
    return weapon;
}

IWeapon GetWeapon(string name)
{
    if (!weaponCatalog.TryGetValue(name, out var weapon))
    {
        throw new InvalidOperationException($"Unknown weapon '{name}'.");
    }

    return weapon;
}

IArmor RegisterArmor(IArmor armor)
{
    armorCatalog[armor.Name] = armor;
    return armor;
}

IArmor GetArmor(string name)
{
    if (!armorCatalog.TryGetValue(name, out var armor))
    {
        throw new InvalidOperationException($"Unknown armor '{name}'.");
    }

    return armor;
}

void RegisterWeapons()
{
    var weapons = new IWeapon[]
    {
        new Weapon(name: "Fists", damage: 1),
        new Weapon(name: "Stick", damage: 1),
        new Weapon(name: "Dagger", damage: 5),
        new Weapon(name: "Short Sword", damage: 7),
        new Weapon(name: "War Axe", damage: 9),
        new SelfDamagingWeapon(name: "Morningstar Flail", damage: 12, selfDamage: 1),
        new Weapon(name: "Wizard Staff", damage: 11),
        new Weapon(name: "Rusty Blade", damage: 4),
        new Weapon(name: "Fangs", damage: 5),
        new Weapon(name: "Shiv", damage: 6),
        new Weapon(name: "Heavy Club", damage: 7),
        new Weapon(name: "Ancient Sword", damage: 6),
        new Weapon(name: "Shadow Bow", damage: 8),
        new Weapon(name: "Stone Hammer", damage: 9),
        new Weapon(name: "Flame Breath", damage: 12),
        new Weapon(name: "Soul Drain", damage: 15)
    };

    foreach (var weapon in weapons)
    {
        RegisterWeapon(weapon);
    }
}

void RegisterArmors()
{
    var armors = new IArmor[]
    {
        new Armor(name: "Cloth Shirt", defense: 1),
        new Armor(name: "Padded Vest", defense: 2),
        new Armor(name: "Chain Shirt", defense: 4),
        new Armor(name: "Enchanted Robes", defense: 5),
        new Armor(name: "Plate Mail", defense: 6),
        new Armor(name: "Thick Glasses", defense: 1),
        new Armor(name: "Leather Scraps", defense: 2),
        new Armor(name: "Matted Fur", defense: 1),
        new Armor(name: "Patchwork Vest", defense: 2),
        new Armor(name: "Chain Vest", defense: 3),
        new Armor(name: "Bone Plating", defense: 2),
        new Armor(name: "Hooded Cloak", defense: 3),
        new Armor(name: "Thick Hide", defense: 4),
        new Armor(name: "Scale Hide", defense: 4),
        new Armor(name: "Shadow Shroud", defense: 5)
    };

    foreach (var armor in armors)
    {
        RegisterArmor(armor);
    }
}

RegisterWeapons();
RegisterArmors();

Player CreateNewPlayer()
{
    return new Player(
        name: "Jackson",
        level: 1,
        health: 100,
        weapon: GetWeapon("Fists"),
        armor: GetArmor("Cloth Shirt"),
        gold: 1000u);
}

var weaponOffers = new List<(IWeapon weapon, uint cost)>
{
    (GetWeapon("Dagger"), 30u),
    (GetWeapon("Short Sword"), 55u),
    (GetWeapon("War Axe"), 80u),
    (GetWeapon("Morningstar Flail"), 110u),
    (GetWeapon("Wizard Staff"), 100u)
};

var armorOffers = new List<(IArmor armor, uint cost)>
{
    (GetArmor("Padded Vest"), 35u),
    (GetArmor("Chain Shirt"), 60u),
    (GetArmor("Enchanted Robes"), 75u),
    (GetArmor("Plate Mail"), 90u)
};

var blacksmith = new Blacksmith(weaponOffers);
var armorer = new Armorer(armorOffers);
var healersHut = new HealersHut();
var saveGameService = new SaveGameService(weaponCatalog, armorCatalog);

var levelOneEnemies = new List<Fighter>
{
    new Fighter(name: "Nerd", health: 120, weapon: GetWeapon("Stick"), armor: GetArmor("Thick Glasses"), gold: 5u),
    new Fighter(name: "Goblin", health: 80, weapon: GetWeapon("Rusty Blade"), armor: GetArmor("Leather Scraps"), gold: 12u),
    new Fighter(name: "Rabid Dog", health: 70, weapon: GetWeapon("Fangs"), armor: GetArmor("Matted Fur"), gold: 8u),
    new Fighter(name: "Bandit", health: 90, weapon: GetWeapon("Shiv"), armor: GetArmor("Patchwork Vest"), gold: 15u)
};

var levelTwoEnemies = new List<Fighter>
{
    new Fighter(name: "Orc Warrior", health: 140, weapon: GetWeapon("Heavy Club"), armor: GetArmor("Chain Vest"), gold: 20u),
    new Fighter(name: "Skeleton Knight", health: 110, weapon: GetWeapon("Ancient Sword"), armor: GetArmor("Bone Plating"), gold: 15u),
    new Fighter(name: "Dark Archer", health: 100, weapon: GetWeapon("Shadow Bow"), armor: GetArmor("Hooded Cloak"), gold: 18u),
    new Fighter(name: "Troll Bruiser", health: 160, weapon: GetWeapon("Stone Hammer"), armor: GetArmor("Thick Hide"), gold: 25u)
};

var levelContents = new Dictionary<int, LevelContent>
{
    [1] = new LevelContent(levelOneEnemies, new Boss(name: "Dragon Whelp", level: 1, health: 200, weapon: GetWeapon("Flame Breath"), armor: GetArmor("Scale Hide"), gold: 50u)),
    [2] = new LevelContent(levelTwoEnemies, new Boss(name: "Lich Lord", level: 2, health: 260, weapon: GetWeapon("Soul Drain"), armor: GetArmor("Shadow Shroud"), gold: 75u))
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
