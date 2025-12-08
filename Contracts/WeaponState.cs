using System.Collections.Generic;

namespace fights;

public sealed class WeaponState
{
    public WeaponState()
    {
        ModifierStates = new List<string?>();
    }

    public WeaponState(string templateName, string name, int damage, IReadOnlyList<string?> modifierStates)
    {
        TemplateName = templateName;
        Name = name;
        Damage = damage;
        ModifierStates = new List<string?>(modifierStates ?? new List<string?>());
    }

    public string TemplateName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Damage { get; set; }
    public List<string?> ModifierStates { get; set; }
}
