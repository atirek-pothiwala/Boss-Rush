using UnityEngine;

public class Constants
{
    private static Constants instance;

    public static Constants Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Constants();
            }

            return instance;
        }
    }

    public static readonly string[] HeroNames = { "Samurai", "Shinobi", "Fighter" };

    private Constants() { }

    private int level = 0;
    private int selectedHeroIndex = 0;
    private const int maxLevel = 2;

    public int CurrentLevel => level;
    public int SelectedHeroIndex => selectedHeroIndex;
    public bool HasMoreBosses => level < maxLevel;
    public bool IsNextLevel => HasMoreBosses;

    public void SelectHero(int heroIndex)
    {
        selectedHeroIndex = Mathf.Clamp(heroIndex, 0, HeroNames.Length - 1);
    }

    public void ResetProgress()
    {
        level = 0;
        GameSave.ClearRun();
    }

    public void LoadProgress(int heroIndex, int savedLevel)
    {
        SelectHero(heroIndex);
        level = Mathf.Clamp(savedLevel, 0, maxLevel);
    }

    public void NextLevel()
    {
        if (level + 1 > maxLevel) return;
        level += 1;
        GameSave.SaveRun(selectedHeroIndex, level);
    }

    public void PersistRun()
    {
        GameSave.SaveRun(selectedHeroIndex, level);
    }

    public string BossName()
    {
        return BossNameForLevel(level);
    }

    public string NextBossName()
    {
        if (!HasMoreBosses) return "";
        return BossNameForLevel(level + 1);
    }

    public string GetBossDefeatedStatusMessage()
    {
        if (HasMoreBosses)
        {
            return $"{BossName()} defeated!\nNext boss: {NextBossName()}";
        }

        return $"Victory!\n{SelectedHeroName()} conquered the Boss Rush!";
    }

    public void CompleteRun()
    {
        GameSave.ClearRun();
    }

    private static string BossNameForLevel(int bossLevel)
    {
        switch(bossLevel) {
            case 0:
            return "Minotaur";
            case 1:
            return "Werewolf";
            case 2:
            return "Gorgon";
        }
        return "";
    }

    public string SelectedHeroName()
    {
        return HeroNames[selectedHeroIndex];
    }
}
