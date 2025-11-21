using System;
using System.Collections.Generic;
using System.IO;

namespace fights;

public class SaveGameService
{
    private readonly string _saveFilePath;

    public SaveGameService(string? saveFilePath = null)
    {
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
            $"WeaponDamage={player.Weapon.Damage}",
            $"ArmorName={player.Armor.Name}",
            $"ArmorDefense={player.Armor.Defense}",
            $"Gold={player.Gold}"
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
            var weaponDamage = GetInt(data, "WeaponDamage");
            var armorName = GetString(data, "ArmorName");
            var armorDefense = GetInt(data, "ArmorDefense");
            var gold = GetUint(data, "Gold");

            var weapon = new Weapon(weaponName, weaponDamage);
            var armor = new Armor(armorName, armorDefense);
            var player = new Player(name, level, maxHealth, weapon, armor, gold);
            player.RestoreHealth(health, maxHealth);

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
}
