using System;
using System.Collections.Generic;

namespace fights;

public sealed class PlayerTemplate
{
    private readonly Dictionary<string, string> _data;

    public PlayerTemplate(Dictionary<string, string> data)
    {
        _data = new Dictionary<string, string>(data, StringComparer.OrdinalIgnoreCase);

        CharacterName = PlayerDataParser.GetRequiredString(_data, "Name");
        Title = PlayerDataParser.GetOptionalString(_data, "TemplateName") ?? CharacterName;
        Level = PlayerDataParser.GetRequiredInt(_data, "Level");
        MaxHealth = PlayerDataParser.GetRequiredInt(_data, "MaxHealth");
        Health = PlayerDataParser.GetRequiredInt(_data, "Health");
        if (Health < 0 || Health > MaxHealth)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Health must be between 0 and max health.");
        }

        WeaponName = PlayerDataParser.GetRequiredString(_data, "WeaponName");
        ArmorName = PlayerDataParser.GetRequiredString(_data, "ArmorName");
        Gold = PlayerDataParser.GetRequiredUint(_data, "Gold");
        PoisonPreview = PlayerDataParser.TryGetPoisonState(_data);
    }

    public string Title { get; }
    public string CharacterName { get; }
    public int Level { get; }
    public int Health { get; }
    public int MaxHealth { get; }
    public string WeaponName { get; }
    public string ArmorName { get; }
    public uint Gold { get; }
    public PoisonState? PoisonPreview { get; }

    public Player CreatePlayer(Func<string, IWeapon> weaponResolver, Func<string, IArmor> armorResolver)
    {
        return PlayerDataParser.CreatePlayer(new Dictionary<string, string>(_data, StringComparer.OrdinalIgnoreCase), weaponResolver, armorResolver);
    }

    public override string ToString() => Title;
}
