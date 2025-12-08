using System;

namespace fights;

public class BreakableWeapon : Weapon
{
    private readonly int _breakChance;
    private readonly string _brokenName;
    private readonly int _brokenDamage;
    private readonly string _intactName;
    private bool _isBroken;

    public BreakableWeapon(string name, int damage, int breakChance, string brokenName, int brokenDamage)
        : base(name, damage)
    {
        if (breakChance < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(breakChance), "Break chance must be at least 1.");
        }

        if (string.IsNullOrWhiteSpace(brokenName))
        {
            throw new ArgumentException("Broken name must be provided.", nameof(brokenName));
        }

        if (brokenDamage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(brokenDamage), "Broken damage must be at least 1.");
        }

        _breakChance = breakChance;
        _brokenName = brokenName;
        _brokenDamage = brokenDamage;
        _intactName = name;
    }

    public override IDamagePayload CreateDamagePayload()
    {
        var payload = base.CreateDamagePayload();

        if (!_isBroken && ShouldBreak())
        {
            BreakWeapon();
        }

        return payload;
    }

    private bool ShouldBreak() => Random.Shared.Next(0, _breakChance) == 0;

    private void BreakWeapon()
    {
        _isBroken = true;
        Name = _brokenName;
        Damage = _brokenDamage;
        Console.WriteLine($"{_intactName} shatters! It's now {_brokenName}.");
    }
}
