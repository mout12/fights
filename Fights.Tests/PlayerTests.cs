using fights;

namespace Fights.Tests;

[TestClass]
public sealed class PlayerTests
{
    [TestMethod]
    public void LevelUp_IncrementsPlayerLevel()
    {
        var weapon = new Weapon("Starter Sword", 10);
        var armor = new Armor("Leather Armor", 2);
        var player = new Player("Rookie", 1, 100, weapon, armor, 25);

        player.LevelUp();

        Assert.AreEqual(2, player.Level);
    }
}
