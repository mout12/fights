using fights;

namespace Fights.Tests;

[TestClass]
[DoNotParallelize]
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

    [TestMethod]
    public void FaeryEncounter_HealsPlayerToFullHealth()
    {
        var weapon = new Weapon("Starter Sword", 10);
        var armor = new Armor("Leather Armor", 2);
        GameRandom.Current = new FixedRandom(value: 1); // skip mitigation
        var player = new Player("Rookie", 1, 100, weapon, armor, 25);
        player.TakeDamage(new DamagePayload(40, 0, false));

        var level = new LevelContent(new List<Fighter> { new("Target", 10, weapon, armor, 0) }, new Boss("Boss", 1, 10, weapon, armor, 0));

        new FaeryEncounter().Execute(player, level, new FixedSelectionService<bool>(true));

        Assert.AreEqual(player.MaxHealth, player.Health);
    }

    [TestMethod]
    public void MysteryBoxEncounter_WhenSafeAndOpened_AwardsGold()
    {
        var weapon = new Weapon("Starter Sword", 10);
        var armor = new Armor("Leather Armor", 2);
        var player = new Player("Rookie", 1, 100, weapon, armor, 25);
        var level = new LevelContent(new List<Fighter> { new("Target", 10, weapon, armor, 0) }, new Boss("Boss", 1, 10, weapon, armor, 0));
        GameRandom.Current = new FixedRandom(value: 100); // avoid trap and award max configured gold

        var continued = new MysteryBoxEncounter().Execute(player, level, new FixedSelectionService<bool>(true));

        Assert.IsTrue(continued);
        Assert.AreEqual<uint>(125, player.Gold);
    }

    [TestMethod]
    public void MysteryBoxEncounter_WhenLeftAlone_DoesNotAwardGold()
    {
        var weapon = new Weapon("Starter Sword", 10);
        var armor = new Armor("Leather Armor", 2);
        var player = new Player("Rookie", 1, 100, weapon, armor, 25);
        var level = new LevelContent(new List<Fighter> { new("Target", 10, weapon, armor, 0) }, new Boss("Boss", 1, 10, weapon, armor, 0));

        var continued = new MysteryBoxEncounter().Execute(player, level, new FixedSelectionService<bool>(false));

        Assert.IsTrue(continued);
        Assert.AreEqual<uint>(25, player.Gold);
    }

    private sealed class FixedSelectionService<T> : IInputSelectionService
    {
        private readonly T _selection;

        public FixedSelectionService(T selection)
        {
            _selection = selection;
        }

        public TResult SelectOption<TResult>(string prompt, IReadOnlyList<InputOption<TResult>> options)
        {
            return _selection is TResult result ? result : throw new InvalidOperationException("Unexpected selection type.");
        }
    }
}
