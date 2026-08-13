using UnityEngine;

public static class HeroStats
{
    public static void Apply(int heroIndex, PlayerController controller, HealthManager health)
    {
        switch (Mathf.Clamp(heroIndex, 0, Constants.HeroNames.Length - 1))
        {
            case 0: // Samurai — balanced
                controller.ApplyStatModifiers(walkSpeed: 1.5f, runSpeed: 3f, damageMultiplier: 1f);
                health.SetHeroMaxHealth(100f);
                break;
            case 1: // Shinobi — fast, lighter hits
                controller.ApplyStatModifiers(walkSpeed: 1.85f, runSpeed: 3.7f, damageMultiplier: 0.85f);
                health.SetHeroMaxHealth(90f);
                break;
            case 2: // Fighter — tank, heavy hits
                controller.ApplyStatModifiers(walkSpeed: 1.3f, runSpeed: 2.7f, damageMultiplier: 1.2f);
                health.SetHeroMaxHealth(120f);
                break;
        }
    }
}
