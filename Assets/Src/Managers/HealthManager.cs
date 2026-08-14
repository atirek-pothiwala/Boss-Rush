using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    [SerializeField] [Range(1, 2)] private int regenerationRate = 1;
    [SerializeField] [Range(0.5f, 2f)]  private float regenerationDelay = 1f;

    private float heroMaxHealth = 100f;
    private float bossMaxHealth = 100f;
    private Coroutine regenerationRoutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetValues();
        regenerationRoutine = StartCoroutine(RegenerateLoop());
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (regenerationRoutine != null)
        {
            StopCoroutine(regenerationRoutine);
        }
    }

    private IEnumerator RegenerateLoop()
    {
        var wait = new WaitForSeconds(regenerationDelay);
        while (true)
        {
            yield return wait;
            if (SceneTransition.IsLoading) yield break;

            var pauseManager = PauseManager.Instance;
            if (pauseManager == null || pauseManager.IsGamePaused) continue;
            if (IsGameOver) continue;

            BossStamina += regenerationRate * 2;
            HeroStamina += regenerationRate;
        }
    }

    public void SetHeroMaxHealth(float maxHealth)
    {
        heroMaxHealth = Mathf.Max(1f, maxHealth);
        HeroHealth = heroMaxHealth;
    }

    public void ResetValues()
    {
        bossMaxHealth = 100f;
        heroMaxHealth = 100f;
        BossHealth = bossMaxHealth;
        BossStamina = 100f;
        HeroHealth = heroMaxHealth;
        HeroStamina = 100f;
    }

    public float BossHealth
    {
        get => GetUiManager()?.BossHealth.value * bossMaxHealth ?? bossMaxHealth;
        private set
        {
            var ui = GetUiManager();
            if (ui != null) ui.BossHealth.value = Mathf.Clamp01(value / bossMaxHealth);
        }
    }

    public float BossStamina
    {
        get => GetUiManager()?.BossStamina.value * 100 ?? 100f;
        private set
        {
            var ui = GetUiManager();
            if (ui != null) ui.BossStamina.value = Mathf.Clamp01(value / 100);
        }
    }

    public float HeroHealth
    {
        get => GetUiManager()?.HeroHealth.value * heroMaxHealth ?? heroMaxHealth;
        private set
        {
            var ui = GetUiManager();
            if (ui != null) ui.HeroHealth.value = Mathf.Clamp01(value / heroMaxHealth);
        }
    }

    public float HeroStamina
    {
        get => GetUiManager()?.HeroStamina.value * 100 ?? 100f;
        private set
        {
            var ui = GetUiManager();
            if (ui != null) ui.HeroStamina.value = Mathf.Clamp01(value / 100);
        }
    }

    public float BossHealthPercent => GetUiManager()?.BossHealth.value ?? 1f;

    private static UIManager GetUiManager() => UIManager.Instance;

    public void UpdateBossHealth(int value)
    {
        BossHealth += value;
        if(BossHealth <= 0) BossStamina = 0;
    }
    public void UpdateBossStamina(int value) => BossStamina += value;

    public void UpdateHeroHealth(int value) {
        HeroHealth += value;
        if(HeroHealth <= 0) HeroStamina = 0;
    }
    public void UpdateHeroStamina(int value) => HeroStamina += value;

    public bool IsBossDead => BossHealth <= 0;
    public bool IsHeroDead => HeroHealth <= 0;

    public bool IsBossStaminaDepleted => BossStamina <= 0;
    public bool IsHeroStaminaDepleted => HeroStamina <= 0;

    public bool IsGameOver => IsHeroDead || IsBossDead;
}
