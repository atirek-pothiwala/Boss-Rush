using UnityEngine;

public static class GameSave
{
    private const string HeroKey = "bossrush.hero";
    private const string LevelKey = "bossrush.level";
    private const string MusicVolumeKey = "bossrush.musicVolume";
    private const string SfxVolumeKey = "bossrush.sfxVolume";

    public static void SaveRun(int heroIndex, int level)
    {
        PlayerPrefs.SetInt(HeroKey, heroIndex);
        PlayerPrefs.SetInt(LevelKey, level);
        PlayerPrefs.Save();
    }

    public static bool TryLoadRun(out int heroIndex, out int level)
    {
        if (!PlayerPrefs.HasKey(LevelKey))
        {
            heroIndex = 0;
            level = 0;
            return false;
        }

        heroIndex = PlayerPrefs.GetInt(HeroKey, 0);
        level = PlayerPrefs.GetInt(LevelKey, 0);
        return true;
    }

    public static void ClearRun()
    {
        PlayerPrefs.DeleteKey(HeroKey);
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.Save();
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        set
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }

    public static float SfxVolume
    {
        get => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        set
        {
            PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
            PlayerPrefs.Save();
        }
    }
}
