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

var dataLoader = new DataLoadingService();
var inputSelector = new ConsoleInputSelectionService();

var weaponDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Weapons.txt");
foreach (var weapon in dataLoader.LoadWeapons(weaponDataPath))
{
    RegisterWeapon(weapon);
}

var armorDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Armor.txt");
foreach (var armor in dataLoader.LoadArmors(armorDataPath))
{
    RegisterArmor(armor);
}

var blacksmithDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Blacksmith.txt");
var weaponOffers = dataLoader.LoadBlacksmithOffers(blacksmithDataPath, GetWeapon);
var armorerDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Armorer.txt");
var armorOffers = dataLoader.LoadArmorerOffers(armorerDataPath, GetArmor);
var levelsDataPath = Path.Combine(AppContext.BaseDirectory, "Data", "Levels.txt");
var levelSetups = dataLoader.LoadLevelSetups(levelsDataPath, GetWeapon, GetArmor);

Player CreateNewPlayer()
{
    var newGamePath = Path.Combine(AppContext.BaseDirectory, "Data", "NewGame.txt");
    return dataLoader.LoadPlayerTemplate(newGamePath, GetWeapon, GetArmor);
}

var blacksmith = new Blacksmith(weaponOffers, inputSelector);
var armorer = new Armorer(armorOffers, inputSelector);
var healersHut = new HealersHut(inputSelector);
var saveGameService = new SaveGameService(weaponCatalog, armorCatalog);

var levelContents = new Dictionary<int, LevelContent>(levelSetups.Count);
foreach (var setup in levelSetups)
{
    levelContents[setup.Level] = new LevelContent(setup.Enemies, setup.Boss);
}

var player = InitializePlayer(saveGameService);
var town = new Town(player, blacksmith, armorer, healersHut, levelContents, inputSelector);
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
        Console.WriteLine("Starting a new adventure.");
        return CreateNewPlayer();
    }

    var selection = inputSelector.SelectOption("A previous adventure was found. Choose an option:", new[]
    {
        new InputOption<Func<Player>>(
            "Start over",
            () =>
            {
                Console.WriteLine("Starting a new adventure.");
                return CreateNewPlayer();
            }),
        new InputOption<Func<Player>>(
            "Load previous adventure",
            () =>
            {
                var loadedPlayer = saveService.TryLoad();
                if (loadedPlayer is not null)
                {
                    Console.WriteLine("Previous adventure loaded.");
                    return loadedPlayer;
                }

                Console.WriteLine("Failed to load the previous adventure. Starting fresh.");
                return CreateNewPlayer();
            })
    });

    return selection();
}
