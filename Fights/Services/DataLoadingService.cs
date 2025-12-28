using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace fights;

public class DataLoadingService
{
    public List<IWeapon> LoadWeapons(string filePath)
    {
        var fullPath = EnsureFileExists(filePath, "Weapon data file");
        var weapons = new List<IWeapon>();

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
                weapons.Add(weapon);
            }
        }

        return weapons;
    }

    public List<IArmor> LoadArmors(string filePath)
    {
        var fullPath = EnsureFileExists(filePath, "Armor data file");
        var armors = new List<IArmor>();

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

            armors.Add(new Armor(name, defense));
        }

        return armors;
    }

    public List<(IWeapon weapon, uint cost)> LoadBlacksmithOffers(string filePath, Func<string, IWeapon> weaponResolver)
    {
        ArgumentNullException.ThrowIfNull(weaponResolver);

        var fullPath = EnsureFileExists(filePath, "Blacksmith data file");
        var offers = new List<(IWeapon weapon, uint cost)>();

        foreach (var line in FilterDataLines(File.ReadLines(fullPath)))
        {
            var parts = line.Split(',', 3, StringSplitOptions.TrimEntries);
            if (parts.Length < 2)
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

            var stateJson = parts.Length == 3 ? parts[2] : null;

            try
            {
                var weapon = weaponResolver(weaponName);
                if (!string.IsNullOrWhiteSpace(stateJson) && !TryApplyWeaponState(weapon, stateJson))
                {
                    Console.WriteLine($"Skipping weapon state for '{weaponName}' due to invalid data.");
                }

                offers.Add((weapon, cost));
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Skipping blacksmith offer because weapon '{weaponName}' is not registered.");
            }
        }

        return offers;
    }

    public List<(IArmor armor, uint cost)> LoadArmorerOffers(string filePath, Func<string, IArmor> armorResolver)
    {
        ArgumentNullException.ThrowIfNull(armorResolver);

        var fullPath = EnsureFileExists(filePath, "Armorer data file");
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
                offers.Add((armorResolver(armorName), cost));
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Skipping armorer offer because armor '{armorName}' is not registered.");
            }
        }

        return offers;
    }

    public List<PlayerTemplate> LoadPlayerTemplates(string filePath, Func<string, IWeapon> weaponResolver, Func<string, IArmor> armorResolver)
    {
        ArgumentNullException.ThrowIfNull(weaponResolver);
        ArgumentNullException.ThrowIfNull(armorResolver);

        var fullPath = EnsureFileExists(filePath, "New game template");
        var sections = ParsePlayerTemplateEntries(File.ReadLines(fullPath));
        if (sections.Count == 0)
        {
            throw new InvalidDataException("No player templates were found in the new game data.");
        }

        var templates = new List<PlayerTemplate>(sections.Count);
        foreach (var section in sections)
        {
            try
            {
                var template = CreatePlayerTemplate(section, weaponResolver, armorResolver);
                templates.Add(template);
            }
            catch (Exception ex)
            {
                var identifier = section.TryGetValue("TemplateName", out var templateName) && !string.IsNullOrWhiteSpace(templateName)
                    ? templateName
                    : section.TryGetValue("Name", out var name) ? name : "<unknown>";
                Console.WriteLine($"Skipping player template '{identifier}': {ex.Message}");
            }
        }

        if (templates.Count == 0)
        {
            throw new InvalidDataException("No valid player templates were found in the new game data.");
        }

        return templates;
    }

    public List<LevelSetup> LoadLevelSetups(string filePath, Func<string, IWeapon> weaponResolver, Func<string, IArmor> armorResolver)
    {
        ArgumentNullException.ThrowIfNull(weaponResolver);
        ArgumentNullException.ThrowIfNull(armorResolver);

        var fullPath = EnsureFileExists(filePath, "Level data file");
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
                ParseEnemyEntry(level, parts, line, enemiesByLevel, weaponResolver, armorResolver);
            }
            else if (entryType.Equals("Boss", StringComparison.OrdinalIgnoreCase))
            {
                ParseBossEntry(level, parts, line, bossesByLevel, weaponResolver, armorResolver);
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

    private static string EnsureFileExists(string filePath, string description)
    {
        var fullPath = Path.GetFullPath(filePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"{description} '{fullPath}' was not found.");
        }

        return fullPath;
    }

    private const string LegacySelfDamagingType = "SelfDamagingWeapon";
    private const string LegacyBreakableType = "BreakableWeapon";

    private static IWeapon? CreateWeaponFromParts(string type, string name, int damage, string[] parts)
    {
        var modifiers = new List<IWeaponModifier>();
        var nextModifierIndex = 3;

        if (type.Equals(LegacySelfDamagingType, StringComparison.OrdinalIgnoreCase))
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

            modifiers.Add(new SelfDamageWeaponModifier(selfDamage));
            nextModifierIndex = 4;
            type = nameof(Weapon);
        }

        if (type.Equals(LegacyBreakableType, StringComparison.OrdinalIgnoreCase))
        {
            if (parts.Length < 6)
            {
                Console.WriteLine($"Skipping breakable weapon '{name}' because required data is missing.");
                return null;
            }

            if (!int.TryParse(parts[3], out var breakChance))
            {
                Console.WriteLine($"Skipping breakable weapon '{name}' with invalid break chance: '{parts[3]}'");
                return null;
            }

            var brokenNameLegacy = parts[4];
            if (string.IsNullOrWhiteSpace(brokenNameLegacy))
            {
                Console.WriteLine($"Skipping breakable weapon '{name}' because broken name is missing.");
                return null;
            }

            if (!int.TryParse(parts[5], out _))
            {
                Console.WriteLine($"Skipping breakable weapon '{name}' with invalid broken damage: '{parts[5]}'");
                return null;
            }

            modifiers.Add(new BreakableWeaponModifier(breakChance, brokenNameLegacy));
            nextModifierIndex = 6;
            type = nameof(Weapon);
        }

        if (!type.Equals(nameof(Weapon), StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Skipping weapon '{name}' with unknown type '{type}'.");
            return null;
        }

        for (var i = nextModifierIndex; i < parts.Length; i++)
        {
            var descriptor = parts[i];
            if (string.IsNullOrWhiteSpace(descriptor))
            {
                continue;
            }

            if (TryCreateModifierFromDescriptor(descriptor, out var modifier))
            {
                modifiers.Add(modifier!);
            }
            else
            {
                Console.WriteLine($"Skipping modifier '{descriptor}' for weapon '{name}' due to invalid format.");
            }
        }

        return new Weapon(name, damage, modifiers);
    }

    private static bool TryCreateModifierFromDescriptor(string descriptor, out IWeaponModifier? modifier)
    {
        modifier = null;

        var modifierParts = descriptor.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (modifierParts.Length == 0)
        {
            return false;
        }

        var modifierType = modifierParts[0];
        if (modifierType.Equals("SelfDamage", StringComparison.OrdinalIgnoreCase))
        {
            if (modifierParts.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(modifierParts[1], out var selfDamage) || selfDamage < 0)
            {
                return false;
            }

            modifier = new SelfDamageWeaponModifier(selfDamage);
            return true;
        }

        if (modifierType.Equals("Breakable", StringComparison.OrdinalIgnoreCase))
        {
            if (modifierParts.Length < 3)
            {
                return false;
            }

            if (!int.TryParse(modifierParts[1], out var breakChance) || breakChance < 1)
            {
                return false;
            }

            var brokenWeaponKey = modifierParts[2];
            if (string.IsNullOrWhiteSpace(brokenWeaponKey))
            {
                return false;
            }

            modifier = new BreakableWeaponModifier(breakChance, brokenWeaponKey);
            return true;
        }

        if (modifierType.Equals("RepairTo", StringComparison.OrdinalIgnoreCase))
        {
            if (modifierParts.Length != 2)
            {
                return false;
            }

            var repairedWeaponKey = modifierParts[1];
            if (string.IsNullOrWhiteSpace(repairedWeaponKey))
            {
                return false;
            }

            modifier = new RepairWeaponModifier(repairedWeaponKey);
            return true;
        }

        if (modifierType.Equals("DoubleStrike", StringComparison.OrdinalIgnoreCase))
        {
            if (modifierParts.Length != 1)
            {
                return false;
            }

            modifier = new DoubleStrikeWeaponModifier();
            return true;
        }

        if (modifierType.Equals("Poison", StringComparison.OrdinalIgnoreCase))
        {
            if (modifierParts.Length != 5)
            {
                return false;
            }

            if (!int.TryParse(modifierParts[1], out var applyChance) || applyChance < 1 || applyChance > 100)
            {
                return false;
            }

            if (!int.TryParse(modifierParts[2], out var tickChance) || tickChance < 1 || tickChance > 100)
            {
                return false;
            }

            if (!int.TryParse(modifierParts[3], out var damagePerTurn) || damagePerTurn < 1)
            {
                return false;
            }

            if (!int.TryParse(modifierParts[4], out var durationTurns) || durationTurns < 1)
            {
                return false;
            }

            modifier = new PoisonWeaponModifier(applyChance, tickChance, damagePerTurn, durationTurns);
            return true;
        }

        return false;
    }

    private static void ParseEnemyEntry(
        int level,
        string[] parts,
        string rawLine,
        Dictionary<int, List<Fighter>> enemiesByLevel,
        Func<string, IWeapon> weaponResolver,
        Func<string, IArmor> armorResolver)
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
            var fighter = new Fighter(name, health, weaponResolver(weaponName), armorResolver(armorName), gold);
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

    private static void ParseBossEntry(
        int level,
        string[] parts,
        string rawLine,
        Dictionary<int, Boss> bossesByLevel,
        Func<string, IWeapon> weaponResolver,
        Func<string, IArmor> armorResolver)
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
            bossesByLevel[level] = new Boss(name, bossLevel, health, weaponResolver(weaponName), armorResolver(armorName), gold);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Skipping boss '{name}' for level {level}: {ex.Message}");
        }
    }

    private static bool TryApplyWeaponState(IWeapon weapon, string stateJson)
    {
        try
        {
            var state = JsonSerializer.Deserialize<WeaponState>(stateJson);
            if (state is null)
            {
                return false;
            }

            weapon.RestoreState(state);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to apply weapon state: {ex.Message}");
            return false;
        }
    }

    private static IEnumerable<string> FilterDataLines(IEnumerable<string> lines)
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

    private static Dictionary<string, string> ParseKeyValueLines(IEnumerable<string> lines)
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

    private static List<Dictionary<string, string>> ParsePlayerTemplateEntries(IEnumerable<string> lines)
    {
        var sections = new List<Dictionary<string, string>>();
        var currentSection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawLine in lines)
        {
            var trimmed = rawLine.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                if (currentSection.Count > 0)
                {
                    sections.Add(currentSection);
                    currentSection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                continue;
            }

            if (trimmed.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            var parts = trimmed.Split('=', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]))
            {
                currentSection[parts[0]] = parts[1];
            }
        }

        if (currentSection.Count > 0)
        {
            sections.Add(currentSection);
        }

        return sections;
    }

    private static PlayerTemplate CreatePlayerTemplate(
        Dictionary<string, string> data,
        Func<string, IWeapon> weaponResolver,
        Func<string, IArmor> armorResolver)
    {
        var snapshot = new Dictionary<string, string>(data, StringComparer.OrdinalIgnoreCase);

        var weaponTemplate = PlayerDataParser.GetOptionalString(snapshot, "WeaponTemplate")
            ?? PlayerDataParser.GetRequiredString(snapshot, "WeaponName");
        var armorName = PlayerDataParser.GetRequiredString(snapshot, "ArmorName");

        // Validate references early
        weaponResolver(weaponTemplate);
        armorResolver(armorName);

        return new PlayerTemplate(snapshot);
    }

}
