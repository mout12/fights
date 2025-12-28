using fights;

namespace Fights.Tests;

[TestClass]
public sealed class PlayerTests
{
    private sealed class FixedRandom : IRandomNumberGenerator
    {
        private readonly int _value;

        public FixedRandom(int value)
        {
            _value = value;
        }

        public int Next(int maxExclusive) => Math.Clamp(_value, 0, Math.Max(0, maxExclusive - 1));

        public int Next(int minInclusive, int maxExclusive) => Math.Clamp(_value, minInclusive, Math.Max(minInclusive, maxExclusive - 1));
    }

    [TestMethod]
    public void LevelUp_IncrementsPlayerLevel()
    {
        var weapon = new Weapon("Starter Sword", 10);
        var armor = new Armor("Leather Armor", 2);
        var player = new Player("Rookie", 1, 100, weapon, armor, 25);

        player.LevelUp();

        Assert.AreEqual(2, player.Level);
    }

    [TestMethod]
    public void TakeDamage_ReducesHealthByDamageMinusArmor()
    {
        var weapon = new Weapon("Starter Sword", 10);
        var armor = new Armor("Leather Armor", 5);
        GameRandom.Current = new FixedRandom(value: 0); // force mitigation

        var player = new Player("Rookie", 1, 100, weapon, armor, 25);

        var dmg = new DamagePayload(10, 0, false);
        player.TakeDamage(dmg);
        Assert.AreEqual(95, player.Health);
    }
}
