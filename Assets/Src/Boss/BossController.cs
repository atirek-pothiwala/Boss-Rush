using System.Collections;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int OnActionHash = Animator.StringToHash("OnAction");

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private GameObject hero;
    private SoundManager SoundManager => SoundManager.Instance;
    private PauseManager PauseManager => PauseManager.Instance;
    private HealthManager HealthManager => HealthManager.Instance;
    
    private bool isGrounded = true;
    private bool isBusy = false;
    private bool isAttackInterrupted = false;

    [Header("Movement Settings")]
    [SerializeField] [Range(1.5f, 2.5f)] private float walkSpeed = 2f;
    [SerializeField] [Range(4f, 6f)]private float runSpeed = 5f;
    [SerializeField] private float airControlMultiplier = 0.6f;
    [SerializeField] public float transitionSpeed = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -20f;
    private readonly float minVerticalVelocity = -2f;

    [Header("AI Decision Settings")]
    [SerializeField] private BossAttackConfig[] attacks;
    private BossAttackConfig currentAttack;
    [SerializeField] [Range(0.5f, 1f)] private float retreatDuration = 1f;
    [SerializeField] [Range(0.5f, 2f)]  float damageCooldown = 1f;
    private float RangeScale => transform.localScale.z;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (PauseManager.IsGamePaused) return;
        if (hero == null)
        {
            hero = GameObject.FindGameObjectWithTag("Hero");
            return;
        }
        ApplyGravity();

        if (HealthManager.IsGameOver) return;
        if (isBusy) return;
        if (isAttackInterrupted) return;
        if(currentAttack == null) { DecideAttack(); return; }

        float distance = Vector2.Distance(transform.position, hero.transform.position);
        if (distance <= currentAttack.range * RangeScale) { StartCoroutine(AttackRoutine()); }
        else { ApproachPlayer(); }
    }

    void ApplyGravity()
    {
        if (isGrounded && rigidBody.linearVelocityY < minVerticalVelocity)
            rigidBody.linearVelocityY = minVerticalVelocity;
        else
            rigidBody.linearVelocityY += gravity * Time.deltaTime;
    }

    private void DecideAttack() {
        var filterAttacks = attacks.Where(attack => attack.stamina <= HealthManager.BossStamina).ToArray();
        if (filterAttacks.Length == 0) return;

        int attackIndex = Random.Range(0, filterAttacks.Length);
        currentAttack = filterAttacks[attackIndex];
    }

    private void ApproachPlayer()
    {
        Vector2 direction = LookingDirection();
        LookAtDirection(direction.x);

        UpdateMovementAnimation(currentAttack.isRun ? BossState.Run : BossState.Walk);
        MoveAtDirection(direction.x, currentAttack.isRun ? runSpeed : walkSpeed);
    }


    // Routines

    private IEnumerator AttackRoutine()
    {
        if (HealthManager.IsBossStaminaDepleted) yield break;
        isBusy = true;
        isAttackInterrupted = false;
    
        if (currentAttack.isAerial)
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * minVerticalVelocity * gravity);
            rigidBody.linearVelocityY = jumpVelocity;
        } else
        {
            StopMovement();
        }
        animator.SetInteger(StateHash, (int) currentAttack.state);
        animator.SetTrigger(OnActionHash);

        yield return null;
    }

    public IEnumerator PostAttackRoutine()
    {
        if (isAttackInterrupted) yield break;
        HealthManager.UpdateBossStamina(-currentAttack.stamina);

        StartCoroutine(hero.GetComponent<PlayerController>().DamageRoutine(currentAttack));
        yield return new WaitForSeconds(currentAttack.cooldown);
        yield return StartCoroutine(RetreatRoutine());
    }

    private IEnumerator RetreatRoutine()
    {
        UpdateMovementAnimation(BossState.Run);
        Vector2 direction = LookingDirection();
        LookAtDirection(-direction.x);

        float timer = 0;
        while (timer < retreatDuration)
        {
            timer += Time.deltaTime;

            MoveAtDirection(-direction.x, runSpeed);
            yield return null;
        }

        UpdateMovementAnimation(BossState.Idle);
        LookAtDirection(direction.x);
        StopMovement();

        yield return new WaitForSeconds(retreatDuration);

        currentAttack = null;
        isAttackInterrupted = false;
        isBusy = false;
    }

    public IEnumerator DamageRoutine(PlayerAttackConfig attack)
    {
        if (HealthManager.IsBossDead) yield break;
        SoundManager.PlayOneShot(attack.hitSound);
        SoundManager.PlayOneShot(attack.bloodSound);
        
        isAttackInterrupted = true;
        isBusy = true;

        HealthManager.UpdateBossHealth(-attack.damage);
        
        Vector2 direction = LookingDirection();
        rigidBody.AddForceX(-direction.x * attack.knockbackForce, ForceMode2D.Impulse);
        animator.SetInteger(StateHash, (int) BossState.Hurt);
        animator.SetTrigger(OnActionHash);
    }

    public IEnumerator PostDamageRoutine()
    {
        if(HealthManager.IsBossDead)
        {
            StopMovement();
            animator.SetInteger(StateHash, (int) BossState.Dead);
            SoundManager.PlayVictory();
            yield break;
        }

        yield return new WaitForSeconds(damageCooldown);
        StartCoroutine(RetreatRoutine());
    }
    

    // Helper Methods

    private void UpdateMovementAnimation(BossState state)
    {
        if(state == BossState.Run) 
        {
            animator.SetFloat(MoveHash, 1f);
        }
        else if(state == BossState.Walk)
        {
            animator.SetFloat(MoveHash, 0.5f);
        }
        else
        {
            animator.SetFloat(MoveHash, 0f);
            bool hasIdle = animator.parameters.Any(anim => anim.nameHash == IdleHash);
            if (hasIdle) animator.SetFloat(IdleHash, Random.Range(0, 1));   
        }

        animator.SetInteger(StateHash, (int) state);
    }

    private Vector2 LookingDirection()
    {
        Vector2 heroPosition = hero.transform.position;
        Vector2 bossPosition = transform.position;
        return (heroPosition - bossPosition).normalized;
    }

    private void LookAtDirection(float direction)
    {
        if (direction > 0)
            transform.localScale = new Vector3(2, 2, 2);
        else if (direction < 0)
            transform.localScale = new Vector3(-2, 2, 2);
    }

    private void MoveAtDirection(float direction, float speed)
    {
        float airMultiplier = isGrounded ? 1f : airControlMultiplier;
        Vector2 moveVelocity = new(direction * speed * airMultiplier, rigidBody.linearVelocityY);
        rigidBody.linearVelocity = moveVelocity;
    }

    private void StopMovement()
    {
        rigidBody.linearVelocityX = 0f;
        animator.SetFloat(MoveHash, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }

    void OnDrawGizmosSelected()
    {
        if (hero == null) return;

        Vector2 bossPos = transform.position;
        Vector2 playerPos = hero.transform.position;

        // Combat distance
        if (currentAttack != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bossPos, currentAttack.range * RangeScale);   
        }

        // Direction to player
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bossPos, playerPos);
    }

}