using System;

namespace fights;

public sealed class PlayerTemplate
{
    public PlayerTemplate(
        string title,
        string characterName,
        int level,
        int health,
        int maxHealth,
        string weaponName,
        string armorName,
        uint gold)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Template title must be provided.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(characterName))
        {
            throw new ArgumentException("Character name must be provided.", nameof(characterName));
        }

        if (level < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
        }

        if (maxHealth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxHealth), "Max health must be positive.");
        }

        if (health < 0 || health > maxHealth)
        {
            throw new ArgumentOutOfRangeException(nameof(health), "Health must be between 0 and max health.");
        }

        if (string.IsNullOrWhiteSpace(weaponName))
        {
            throw new ArgumentException("Weapon name must be provided.", nameof(weaponName));
        }

        if (string.IsNullOrWhiteSpace(armorName))
        {
            throw new ArgumentException("Armor name must be provided.", nameof(armorName));
        }

        Title = title;
        CharacterName = characterName;
        Level = level;
        Health = health;
        MaxHealth = maxHealth;
        WeaponName = weaponName;
        ArmorName = armorName;
        Gold = gold;
    }

    public string Title { get; }
    public string CharacterName { get; }
    public int Level { get; }
    public int Health { get; }
    public int MaxHealth { get; }
    public string WeaponName { get; }
    public string ArmorName { get; }
    public uint Gold { get; }

    public Player CreatePlayer(Func<string, IWeapon> weaponResolver, Func<string, IArmor> armorResolver)
    {
        ArgumentNullException.ThrowIfNull(weaponResolver);
        ArgumentNullException.ThrowIfNull(armorResolver);

        var weapon = weaponResolver(WeaponName);
        var armor = armorResolver(ArmorName);
        var player = new Player(CharacterName, Level, MaxHealth, weapon, armor, Gold);
        player.RestoreHealth(Health, MaxHealth);
        return player;
    }

    public override string ToString() => Title;
}
