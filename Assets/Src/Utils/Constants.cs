using System;
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

    private Constants() { }

    private int level = 0;
    private const int maxLevel = 2;

    public int CurrentLevel => level;
    public bool IsNextLevel => level + 1 <= maxLevel;

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
}
