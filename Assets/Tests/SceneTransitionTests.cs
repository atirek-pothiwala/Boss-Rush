using NUnit.Framework;

public class SceneTransitionTests
{
    [TearDown]
    public void TearDown()
    {
        SceneTransition.ResetLoadingFlagForTests();
    }

    [Test]
    public void Load_DoesNotReloadWhenAlreadyLoading()
    {
        SceneTransition.BeginLoadingForTests();

        SceneTransition.Load("Main Menu");

        Assert.IsTrue(SceneTransition.IsLoading);
    }

    [Test]
    public void ResetLoadingFlag_AllowsSubsequentLoads()
    {
        SceneTransition.BeginLoadingForTests();
        SceneTransition.ResetLoadingFlagForTests();

        Assert.IsFalse(SceneTransition.IsLoading);
    }
}
