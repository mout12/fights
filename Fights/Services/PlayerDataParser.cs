using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace fights;

internal static class PlayerDataParser
{
    public static Player CreatePlayer(
        Dictionary<string, string> data,
        Func<string, IWeapon> weaponResolver,
        Func<string, IArmor> armorResolver)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(weaponResolver);
        ArgumentNullException.ThrowIfNull(armorResolver);

        var name = GetRequiredString(data, "Name");
        var level = GetRequiredInt(data, "Level");
        var health = GetRequiredInt(data, "Health");
        var maxHealth = GetRequiredInt(data, "MaxHealth");
        var weaponName = GetRequiredString(data, "WeaponName");
        var weaponTemplate = GetOptionalString(data, "WeaponTemplate") ?? weaponName;
        var weaponStateJson = GetOptionalString(data, "WeaponState");
        var armorName = GetRequiredString(data, "ArmorName");
        var gold = GetRequiredUint(data, "Gold");

        var weapon = weaponResolver(weaponTemplate);
        if (!string.IsNullOrWhiteSpace(weaponStateJson))
        {
            var weaponState = TryDeserializeWeaponState(weaponStateJson);
            if (weaponState is not null)
            {
                weapon.RestoreState(weaponState);
            }
        }
        else if (!string.Equals(weaponName, weapon.TemplateName, StringComparison.OrdinalIgnoreCase))
        {
            weapon.RestoreState(new WeaponState(weapon.TemplateName, weaponName, weapon.Damage, Array.Empty<string?>()));
        }

        var armor = armorResolver(armorName);
        var player = new Player(name, level, maxHealth, weapon, armor, gold);
        player.RestoreHealth(health, maxHealth);
        player.RestorePoison(TryGetPoisonState(data));

        return player;
    }

    public static string GetRequiredString(Dictionary<string, string> data, string key)
    {
        if (!data.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"Missing value for '{key}'.");
        }

        return value;
    }

    public static int GetRequiredInt(Dictionary<string, string> data, string key)
    {
        if (!data.TryGetValue(key, out var value) || !int.TryParse(value, out var result))
        {
            throw new InvalidDataException($"Invalid integer for '{key}'.");
        }

        return result;
    }

    public static uint GetRequiredUint(Dictionary<string, string> data, string key)
    {
        if (!data.TryGetValue(key, out var value) || !uint.TryParse(value, out var result))
        {
            throw new InvalidDataException($"Invalid unsigned integer for '{key}'.");
        }

        return result;
    }

    public static string? GetOptionalString(Dictionary<string, string> data, string key)
    {
        return data.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
    }

    public static int GetOptionalInt(Dictionary<string, string> data, string key, int defaultValue = 0)
    {
        return data.TryGetValue(key, out var value) && int.TryParse(value, out var result)
            ? result
            : defaultValue;
    }

    public static PoisonState? TryGetPoisonState(Dictionary<string, string> data)
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

    public static WeaponState? TryDeserializeWeaponState(string json)
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
}
