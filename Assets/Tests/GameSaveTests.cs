using NUnit.Framework;
using UnityEngine;

public class GameSaveTests
{
    [SetUp]
    public void SetUp()
    {
        GameSave.ClearRun();
    }

    [TearDown]
    public void TearDown()
    {
        GameSave.ClearRun();
    }

    [Test]
    public void SaveAndLoadRun_PersistsHeroAndLevel()
    {
        GameSave.SaveRun(2, 1);
        Assert.IsTrue(GameSave.TryLoadRun(out var hero, out var level));
        Assert.AreEqual(2, hero);
        Assert.AreEqual(1, level);
    }

    [Test]
    public void ClearRun_RemovesSavedProgress()
    {
        GameSave.SaveRun(1, 2);
        GameSave.ClearRun();
        Assert.IsFalse(GameSave.TryLoadRun(out _, out _));
    }

    [Test]
    public void VolumeSettings_ClampsToZeroOne()
    {
        GameSave.MusicVolume = 2f;
        GameSave.SfxVolume = -1f;
        Assert.AreEqual(1f, GameSave.MusicVolume);
        Assert.AreEqual(0f, GameSave.SfxVolume);
    }
}
