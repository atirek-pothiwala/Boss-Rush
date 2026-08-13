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
    public bool IsNextLevel => level + 1 <= maxLevel;

    public void SelectHero(int heroIndex)
    {
        selectedHeroIndex = Mathf.Clamp(heroIndex, 0, HeroNames.Length - 1);
    }

    public void ResetProgress()
    {
        level = 0;
    }

    public void NextLevel()
    {
        if (level + 1 > maxLevel) return;
        level += 1;
    }

    public string BossName()
    {
        switch(level) {
            case 0:
            return "Minatour";
            case 1:
            return "Warewolf";
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
