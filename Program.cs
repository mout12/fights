using System;
using System.Collections.Generic;
using System.IO;
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

void LoadWeaponsFromFile(string filePath)
{
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"Weapon data file '{fullPath}' was not found.");
    }

    foreach (var line in FilterDataLines(File.ReadLines(fullPath)))
    {
        var parts = line.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
        {
            Console.WriteLine($"Skipping invalid weapon entry: '{line}'");
            continue;
        }

        var type = parts[0];
        var name = parts[1];
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine($"Skipping weapon entry with no name: '{line}'");
            continue;
        }

        if (!int.TryParse(parts[2], out var damage))
        {
            Console.WriteLine($"Skipping weapon '{name}' with invalid damage: '{parts[2]}'");
            continue;
        }

        var weapon = CreateWeaponFromParts(type, name, damage, parts);
        if (weapon is not null)
        {
            RegisterWeapon(weapon);
        }
    }
}

IWeapon? CreateWeaponFromParts(string type, string name, int damage, string[] parts)
{
    if (type.Equals(nameof(Weapon), StringComparison.OrdinalIgnoreCase))
    {
        return new Weapon(name, damage);
    }

    if (type.Equals(nameof(SelfDamagingWeapon), StringComparison.OrdinalIgnoreCase))
    {
        if (parts.Length < 4)
        {
            Console.WriteLine($"Skipping self damaging weapon '{name}' because self damage is missing.");
            return null;
        }

        if (!int.TryParse(parts[3], out var selfDamage))
        {
            Console.WriteLine($"Skipping self damaging weapon '{name}' with invalid self damage: '{parts[3]}'");
            return null;
        }

        return new SelfDamagingWeapon(name, damage, selfDamage);
    }

    Console.WriteLine($"Skipping weapon '{name}' with unknown type '{type}'.");
    return null;
}

void LoadArmorsFromFile(string filePath)
{
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"Armor data file '{fullPath}' was not found.");
    }

    foreach (var line in FilterDataLines(File.ReadLines(fullPath)))
    {
        var parts = line.Split(',', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            Console.WriteLine($"Skipping invalid armor entry: '{line}'");
            continue;
        }

        var name = parts[0];
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine($"Skipping armor entry with no name: '{line}'");
            continue;
        }

        if (!int.TryParse(parts[1], out var defense))
        {
            Console.WriteLine($"Skipping armor '{name}' with invalid defense: '{parts[1]}'");
            continue;
        }

        RegisterArmor(new Armor(name, defense));
    }
}

IEnumerable<string> FilterDataLines(IEnumerable<string> lines)
{
    foreach (var line in lines)
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
        {
            continue;
        }

        yield return trimmed;
    }
}

List<(IWeapon weapon, uint cost)> LoadBlacksmithOffers(string filePath)
{
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"Blacksmith data file '{fullPath}' was not found.");
    }

    var offers = new List<(IWeapon weapon, uint cost)>();
    foreach (var line in FilterDataLines(File.ReadLines(fullPath)))
    {
        var parts = line.Split(',', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            Console.WriteLine($"Skipping invalid blacksmith offer: '{line}'");
            continue;
        }

        var weaponName = parts[0];
        if (string.IsNullOrWhiteSpace(weaponName))
        {
            Console.WriteLine($"Skipping blacksmith offer with no weapon name: '{line}'");
            continue;
        }

        if (!uint.TryParse(parts[1], out var cost))
        {
            Console.WriteLine($"Skipping blacksmith offer for '{weaponName}' with invalid cost: '{parts[1]}'");
            continue;
        }

        try
        {
            offers.Add((GetWeapon(weaponName), cost));
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Skipping blacksmith offer because weapon '{weaponName}' is not registered.");
        }
    }

    return offers;
}

List<(IArmor armor, uint cost)> LoadArmorerOffers(string filePath)
{
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"Armorer data file '{fullPath}' was not found.");
    }

    var offers = new List<(IArmor armor, uint cost)>();
    foreach (var line in FilterDataLines(File.ReadLines(fullPath)))
    {
        var parts = line.Split(',', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            Console.WriteLine($"Skipping invalid armorer offer: '{line}'");
            continue;
        }

        var armorName = parts[0];
        if (string.IsNullOrWhiteSpace(armorName))
        {
            Console.WriteLine($"Skipping armorer offer with no armor name: '{line}'");
            continue;
        }

        if (!uint.TryParse(parts[1], out var cost))
        {
            Console.WriteLine($"Skipping armorer offer for '{armorName}' with invalid cost: '{parts[1]}'");
            continue;
        }

        try
        {
            offers.Add((GetArmor(armorName), cost));
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"Skipping armorer offer because armor '{armorName}' is not registered.");
        }
    }

    return offers;
}

List<LevelSetup> LoadLevelSetups(string filePath)
{
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"Level data file '{fullPath}' was not found.");
    }

    var enemiesByLevel = new Dictionary<int, List<Fighter>>();
    var bossesByLevel = new Dictionary<int, Boss>();
    var levelOrder = new List<int>();
    var seenLevels = new HashSet<int>();

    foreach (var line in FilterDataLines(File.ReadLines(fullPath)))
    {
        var parts = line.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
        {
            Console.WriteLine($"Skipping invalid level entry: '{line}'");
            continue;
        }

        if (!int.TryParse(parts[0], out var level) || level < 1)
        {
            Console.WriteLine($"Skipping level entry with invalid level '{parts[0]}': '{line}'");
            continue;
        }

        if (seenLevels.Add(level))
        {
            levelOrder.Add(level);
        }

        var entryType = parts[1];
        if (entryType.Equals("Enemy", StringComparison.OrdinalIgnoreCase))
        {
            ParseEnemyEntry(level, parts, line, enemiesByLevel);
        }
        else if (entryType.Equals("Boss", StringComparison.OrdinalIgnoreCase))
        {
            ParseBossEntry(level, parts, line, bossesByLevel);
        }
        else
        {
            Console.WriteLine($"Skipping level entry with unknown type '{entryType}': '{line}'");
        }
    }

    var setups = new List<LevelSetup>();
    foreach (var level in levelOrder)
    {
        if (!enemiesByLevel.TryGetValue(level, out var enemies) || enemies.Count == 0)
        {
            throw new InvalidDataException($"Level {level} does not have any enemies defined.");
        }

        if (!bossesByLevel.TryGetValue(level, out var boss))
        {
            throw new InvalidDataException($"Level {level} does not have a boss defined.");
        }

        setups.Add(new LevelSetup(level, enemies, boss));
    }

    return setups;
}

void ParseEnemyEntry(int level, string[] parts, string rawLine, Dictionary<int, List<Fighter>> enemiesByLevel)
{
    if (parts.Length != 7)
    {
        Console.WriteLine($"Skipping enemy entry with unexpected format: '{rawLine}'");
        return;
    }

    var name = parts[2];
    if (!int.TryParse(parts[3], out var health))
    {
        Console.WriteLine($"Skipping enemy '{name}' for level {level} with invalid health: '{parts[3]}'");
        return;
    }

    var weaponName = parts[4];
    var armorName = parts[5];
    if (!uint.TryParse(parts[6], out var gold))
    {
        Console.WriteLine($"Skipping enemy '{name}' for level {level} with invalid gold: '{parts[6]}'");
        return;
    }

    try
    {
        var fighter = new Fighter(name, health, GetWeapon(weaponName), GetArmor(armorName), gold);
        if (!enemiesByLevel.TryGetValue(level, out var list))
        {
            list = new List<Fighter>();
            enemiesByLevel[level] = list;
        }

        list.Add(fighter);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Skipping enemy '{name}' for level {level}: {ex.Message}");
    }
}

void ParseBossEntry(int level, string[] parts, string rawLine, Dictionary<int, Boss> bossesByLevel)
{
    if (parts.Length != 8)
    {
        Console.WriteLine($"Skipping boss entry with unexpected format: '{rawLine}'");
        return;
    }

    var name = parts[2];
    if (!int.TryParse(parts[3], out var bossLevel))
    {
        Console.WriteLine($"Skipping boss '{name}' for level {level} with invalid boss level: '{parts[3]}'");
        return;
    }

    if (!int.TryParse(parts[4], out var health))
    {
        Console.WriteLine($"Skipping boss '{name}' for level {level} with invalid health: '{parts[4]}'");
        return;
    }

    var weaponName = parts[5];
    var armorName = parts[6];
    if (!uint.TryParse(parts[7], out var gold))
    {
        Console.WriteLine($"Skipping boss '{name}' for level {level} with invalid gold: '{parts[7]}'");
        return;
    }

    if (bossesByLevel.ContainsKey(level))
    {
        Console.WriteLine($"Level {level} already has a boss defined. Skipping '{name}'.");
        return;
    }

    try
    {
        bossesByLevel[level] = new Boss(name, bossLevel, health, GetWeapon(weaponName), GetArmor(armorName), gold);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Skipping boss '{name}' for level {level}: {ex.Message}");
    }
}

Player LoadPlayerTemplate(string filePath)
{
    var fullPath = Path.GetFullPath(filePath);
    if (!File.Exists(fullPath))
    {
        throw new FileNotFoundException($"New game template '{fullPath}' was not found.");
    }

    var data = ParseKeyValueLines(File.ReadLines(fullPath));

    var name = GetRequiredString(data, "Name");
    var level = GetRequiredInt(data, "Level");
    var health = GetRequiredInt(data, "Health");
    var maxHealth = GetRequiredInt(data, "MaxHealth");
    var weaponName = GetRequiredString(data, "WeaponName");
    var armorName = GetRequiredString(data, "ArmorName");
    var gold = GetRequiredUint(data, "Gold");

    var player = new Player(
        name,
        level,
        maxHealth,
        GetWeapon(weaponName),
        GetArmor(armorName),
        gold);

    player.RestoreHealth(health, maxHealth);
    return player;
}

Dictionary<string, string> ParseKeyValueLines(IEnumerable<string> lines)
{
    var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    foreach (var line in lines)
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
        {
            continue;
        }

        var parts = trimmed.Split('=', 2, StringSplitOptions.TrimEntries);
        if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]))
        {
            data[parts[0]] = parts[1];
        }
    }

    return data;
}

string GetRequiredString(Dictionary<string, string> data, string key)
{
    if (!data.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidDataException($"Missing value for '{key}' in new game template.");
    }

    return value;
}

int GetRequiredInt(Dictionary<string, string> data, string key)
{
    if (!data.TryGetValue(key, out var value) || !int.TryParse(value, out var result))
    {
        throw new InvalidDataException($"Invalid integer for '{key}' in new game template.");
    }

    return result;
}

uint GetRequiredUint(Dictionary<string, string> data, string key)
{
    if (!data.TryGetValue(key, out var value) || !uint.TryParse(value, out var result))
    {
        throw new InvalidDataException($"Invalid unsigned integer for '{key}' in new game template.");
    }

    return result;
}

var weaponDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Weapons.txt");
LoadWeaponsFromFile(weaponDataPath);
var armorDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Armor.txt");
LoadArmorsFromFile(armorDataPath);
var blacksmithDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Blacksmith.txt");
var weaponOffers = LoadBlacksmithOffers(blacksmithDataPath);
var armorerDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Armorer.txt");
var armorOffers = LoadArmorerOffers(armorerDataPath);
var levelsDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Levels.txt");
var levelSetups = LoadLevelSetups(levelsDataPath);

Player CreateNewPlayer()
{
    var newGamePath = Path.Combine(AppContext.BaseDirectory, "Data", "NewGame.txt");
    return LoadPlayerTemplate(newGamePath);
}

var blacksmith = new Blacksmith(weaponOffers);
var armorer = new Armorer(armorOffers);
var healersHut = new HealersHut();
var saveGameService = new SaveGameService(weaponCatalog, armorCatalog);

var levelContents = new Dictionary<int, LevelContent>(levelSetups.Count);
foreach (var setup in levelSetups)
{
    levelContents[setup.Level] = new LevelContent(setup.Enemies, setup.Boss);
}

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

record LevelSetup(int Level, List<Fighter> Enemies, Boss Boss);
