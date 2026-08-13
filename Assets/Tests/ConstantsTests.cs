using NUnit.Framework;

public class ConstantsTests
{
    [SetUp]
    public void SetUp()
    {
        Constants.Instance.ResetProgress();
        Constants.Instance.SelectHero(0);
    }

    [Test]
    public void BossName_ReturnsExpectedNamesPerLevel()
    {
        Assert.AreEqual("Minotaur", Constants.Instance.BossName());
        Constants.Instance.NextLevel();
        Assert.AreEqual("Werewolf", Constants.Instance.BossName());
        Constants.Instance.NextLevel();
        Assert.AreEqual("Gorgon", Constants.Instance.BossName());
    }

    [Test]
    public void ResetProgress_ReturnsToFirstBoss()
    {
        Constants.Instance.NextLevel();
        Constants.Instance.NextLevel();
        Constants.Instance.ResetProgress();
        Assert.AreEqual(0, Constants.Instance.CurrentLevel);
    }

    [Test]
    public void NextBossName_ReturnsUpcomingBoss()
    {
        Assert.AreEqual("Werewolf", Constants.Instance.NextBossName());
        Constants.Instance.NextLevel();
        Assert.AreEqual("Gorgon", Constants.Instance.NextBossName());
        Constants.Instance.NextLevel();
        Assert.AreEqual("", Constants.Instance.NextBossName());
    }
}
