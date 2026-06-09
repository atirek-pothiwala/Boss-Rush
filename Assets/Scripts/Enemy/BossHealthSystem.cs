using UnityEngine;
using UnityEngine.Events;

public class BossHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 500;
    [SerializeField] private float invincibilityDuration = 0.3f;
    
    private int currentHealth;
    private float invincibilityTimer;
    private Animator animator;
    private Rigidbody2D rigidBody;
    private BossController bossController;

    public UnityEvent<int, int> OnHealthChanged; // currentHealth, maxHealth
    public UnityEvent OnDeath;
    public UnityEvent OnTakeDamage;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => invincibilityTimer > 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        bossController = GetComponent<BossController>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection = default, float knockbackForce = 0f)
    {
        if (IsInvincible || currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        invincibilityTimer = invincibilityDuration;

        OnTakeDamage?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Apply knockback
        if (knockbackForce > 0f && knockbackDirection != Vector2.zero)
        {
            rigidBody.linearVelocity = knockbackDirection.normalized * knockbackForce;
        }

        // Trigger hurt animation
        animator.SetInteger(Animator.StringToHash("State"), (int)BossState.Hurt);

        // Check if boss is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        bossController.enabled = false;
        animator.SetInteger(Animator.StringToHash("State"), (int)BossState.Dead);
        OnDeath?.Invoke();
    }

    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}
