using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace fights;

public class SaveGameService
{
    private readonly string _saveFilePath;
    private readonly IReadOnlyDictionary<string, IWeapon> _weaponCatalog;
    private readonly IReadOnlyDictionary<string, IArmor> _armorCatalog;

    public SaveGameService(
        IReadOnlyDictionary<string, IWeapon> weaponCatalog,
        IReadOnlyDictionary<string, IArmor> armorCatalog,
        string? saveFilePath = null)
    {
        _weaponCatalog = weaponCatalog ?? throw new ArgumentNullException(nameof(weaponCatalog));
        _armorCatalog = armorCatalog ?? throw new ArgumentNullException(nameof(armorCatalog));
        _saveFilePath = saveFilePath ?? Path.Combine(AppContext.BaseDirectory, "savegame.txt");
    }

    public bool HasSave() => File.Exists(_saveFilePath);

    public void Save(Player player)
    {
        if (player is null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        var lines = new[]
        {
            $"Name={player.Name}",
            $"Level={player.Level}",
            $"Health={player.Health}",
            $"MaxHealth={player.MaxHealth}",
            $"WeaponName={player.Weapon.Name}",
            $"WeaponTemplate={player.Weapon.TemplateName}",
            $"WeaponState={SerializeWeaponState(player.Weapon)}",
            $"ArmorName={player.Armor.Name}",
            $"Gold={player.Gold}",
            $"PoisonTickChance={player.ActivePoison?.TickChancePercent ?? 0}",
            $"PoisonDamage={player.ActivePoison?.DamagePerTurn ?? 0}",
            $"PoisonTurns={player.ActivePoison?.RemainingTurns ?? 0}"
        };

        File.WriteAllLines(_saveFilePath, lines);
    }

    public Player? TryLoad()
    {
        if (!HasSave())
        {
            return null;
        }

        try
        {
            var lines = File.ReadAllLines(_saveFilePath);
            var data = ParseLines(lines);

            var name = GetString(data, "Name");
            var level = GetInt(data, "Level");
            var health = GetInt(data, "Health");
            var maxHealth = GetInt(data, "MaxHealth");
            var weaponName = GetString(data, "WeaponName");
            var weaponTemplate = GetOptionalString(data, "WeaponTemplate") ?? weaponName;
            var weaponStateJson = GetOptionalString(data, "WeaponState");
            var armorName = GetString(data, "ArmorName");
            var gold = GetUint(data, "Gold");

            var weapon = GetWeaponByName(weaponTemplate);
            if (!string.IsNullOrWhiteSpace(weaponStateJson))
            {
                var weaponState = DeserializeWeaponState(weaponStateJson);
                if (weaponState is not null)
                {
                    weapon.RestoreState(weaponState);
                }
            }
            else if (!string.Equals(weaponName, weapon.TemplateName, StringComparison.OrdinalIgnoreCase))
            {
                weapon.RestoreState(new WeaponState(weapon.TemplateName, weaponName, weapon.Damage, Array.Empty<string?>()));
            }

            var armor = GetArmorByName(armorName);
            var player = new Player(name, level, maxHealth, weapon, armor, gold);
            player.RestoreHealth(health, maxHealth);
            player.RestorePoison(TryGetPoisonState(data));

            return player;
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, string> ParseLines(string[] lines)
    {
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split('=', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]))
            {
                data[parts[0]] = parts[1];
            }
        }

        return data;
    }

    private static string GetString(Dictionary<string, string> data, string key)
    {
        if (!data.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"Missing value for '{key}'.");
        }

        return value;
    }

    private static int GetInt(Dictionary<string, string> data, string key)
    {
        if (!data.TryGetValue(key, out var value) || !int.TryParse(value, out var result))
        {
            throw new InvalidDataException($"Invalid integer for '{key}'.");
        }

        return result;
    }

    private static uint GetUint(Dictionary<string, string> data, string key)
    {
        if (!data.TryGetValue(key, out var value) || !uint.TryParse(value, out var result))
        {
            throw new InvalidDataException($"Invalid unsigned integer for '{key}'.");
        }

        return result;
    }

    private static string? GetOptionalString(Dictionary<string, string> data, string key)
    {
        return data.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
    }

    private static int GetOptionalInt(Dictionary<string, string> data, string key, int defaultValue = 0)
    {
        return data.TryGetValue(key, out var value) && int.TryParse(value, out var result)
            ? result
            : defaultValue;
    }

    private static PoisonState? TryGetPoisonState(Dictionary<string, string> data)
    {
        var remainingTurns = GetOptionalInt(data, "PoisonTurns");
        if (remainingTurns <= 0)
        {
            return null;
        }

        var tickChance = GetOptionalInt(data, "PoisonTickChance");
        var damage = GetOptionalInt(data, "PoisonDamage");
        try
        {
            return new PoisonState(tickChance, damage, remainingTurns);
        }
        catch
        {
            return null;
        }
    }

    private static string SerializeWeaponState(IWeapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        var state = weapon.CaptureState();
        return JsonSerializer.Serialize(state);
    }

    private static WeaponState? DeserializeWeaponState(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<WeaponState>(json);
        }
        catch
        {
            return null;
        }
    }

    private IWeapon GetWeaponByName(string name)
    {
        if (!_weaponCatalog.TryGetValue(name, out var weapon))
        {
            throw new InvalidDataException($"Unknown weapon '{name}' in save file.");
        }

        return weapon.Clone();
    }

    private IArmor GetArmorByName(string name)
    {
        if (!_armorCatalog.TryGetValue(name, out var armor))
        {
            throw new InvalidDataException($"Unknown armor '{name}' in save file.");
        }

        return armor;
    }
}
