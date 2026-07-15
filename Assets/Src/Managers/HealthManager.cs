using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    [SerializeField] [Range(1, 2)] private int regenerationRate = 1;
    [SerializeField] [Range(0.5f, 2f)]  private float regenerationDelay = 1f;

    bool regenerating;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetValues();
    }

    void Update()
    {
        if (PauseManager.Instance.IsGamePaused)
            return;

        if (IsGameOver)
        {
            UIManager.Instance.PauseMenu.SetActive(true);
            UIManager.Instance.ResumeButton.SetActive(false);
            return;
        }

        if (!regenerating)
            StartCoroutine(RegenerateRoutine());
    }

    IEnumerator RegenerateRoutine()
    {
        regenerating = true;

        yield return new WaitForSeconds(regenerationDelay);

        BossHealth += regenerationRate;
        BossStamina += regenerationRate * 2;

        HeroStamina += regenerationRate;

        regenerating = false;
    }

    public void ResetValues()
    {
        UIManager.Instance.BossHealth.value = 1;
        UIManager.Instance.BossStamina.value = 1;

        UIManager.Instance.HeroHealth.value = 1;
        UIManager.Instance.HeroStamina.value = 1;
    }

    public float BossHealth
    {
        get => UIManager.Instance.BossHealth.value * 100;
        private set => UIManager.Instance.BossHealth.value = Mathf.Clamp01(value / 100);
    }

    public float BossStamina
    {
        get => UIManager.Instance.BossStamina.value * 100;
        private set => UIManager.Instance.BossStamina.value = Mathf.Clamp01(value / 100);
    }

    public float HeroHealth
    {
        get => UIManager.Instance.HeroHealth.value * 100;
        private set => UIManager.Instance.HeroHealth.value = Mathf.Clamp01(value / 100);
    }

    public float HeroStamina
    {
        get => UIManager.Instance.HeroStamina.value * 100;
        private set => UIManager.Instance.HeroStamina.value = Mathf.Clamp01(value / 100);
    }

    public void UpdateBossHealth(int value) => BossHealth += value;
    public void UpdateBossStamina(int value) => BossStamina += value;

    public void UpdateHeroHealth(int value) => HeroHealth += value;
    public void UpdateHeroStamina(int value) => HeroStamina += value;

    public bool IsBossDead => BossHealth <= 0;
    public bool IsHeroDead => HeroHealth <= 0;

    public bool IsBossStaminaDepleted => BossStamina <= 0;
    public bool IsHeroStaminaDepleted => HeroStamina <= 0;

    public bool IsGameOver => IsHeroDead || IsBossDead;
}