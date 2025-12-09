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

            return PlayerDataParser.CreatePlayer(data, GetWeaponByName, GetArmorByName);
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

    private static string SerializeWeaponState(IWeapon weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);
        var state = weapon.CaptureState();
        return JsonSerializer.Serialize(state);
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
