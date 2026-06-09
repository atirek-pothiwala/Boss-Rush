using UnityEngine;

public class BossAttackSystem : MonoBehaviour
{
    [System.Serializable]
    public class AttackConfig
    {
        public BossState attackState;
        public float cooldown;
        public float range;
        public int damage;
        public float knockbackForce;
        [Tooltip("How close/far to position before attacking")]
        public float preferredDistance;
    }

    [SerializeField] private AttackConfig[] attackConfigs;

    private float[] attackCooldowns;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    public bool CanAttack => GetAvailableAttack() >= 0;

    void Awake()
    {
        attackCooldowns = new float[attackConfigs.Length];
    }

    void Update()
    {
        // Update cooldowns
        for (int i = 0; i < attackCooldowns.Length; i++)
        {
            if (attackCooldowns[i] > 0f)
            {
                attackCooldowns[i] -= Time.deltaTime;
            }
        }
    }

    /// Get the index of an available attack, prioritizing more powerful attacks
    public int GetAvailableAttack()
    {
        // Iterate backwards to prioritize stronger attacks
        for (int i = attackConfigs.Length - 1; i >= 0; i--)
        {
            if (attackCooldowns[i] <= 0f)
            {
                return i;
            }
        }
        return -1;
    }

    /// Execute an attack by index
    public void ExecuteAttack(Animator animator, int attackIndex)
    {
        if (attackIndex < 0 || attackIndex >= attackConfigs.Length)
            return;

        AttackConfig config = attackConfigs[attackIndex];
        
        // Set animation state
        animator.SetInteger(StateHash, (int)config.attackState);
        
        // Trigger attack animation
        animator.SetTrigger(OnAttackHash);
        
        // Start cooldown
        attackCooldowns[attackIndex] = config.cooldown;
    }

    /// Get attack config by state
    public AttackConfig GetAttackConfig(BossState state)
    {
        for (int i = 0; i < attackConfigs.Length; i++)
        {
            if (attackConfigs[i].attackState == state)
                return attackConfigs[i];
        }
        return null;
    }

    /// Check if an attack would hit the target
    public bool CanHitTarget(AttackConfig config, Vector2 targetPosition, Vector2 bossPosition)
    {
        float distance = Vector2.Distance(bossPosition, targetPosition);
        return distance <= config.range;
    }

    /// Get the preferred distance to maintain from target for an attack
    public float GetPreferredDistance(int attackIndex)
    {
        if (attackIndex < 0 || attackIndex >= attackConfigs.Length)
            return 0f;
        return attackConfigs[attackIndex].preferredDistance;
    }

    /// Get all attack configs (for boss AI decision making)
    public AttackConfig[] GetAllAttackConfigs() => attackConfigs;
}
