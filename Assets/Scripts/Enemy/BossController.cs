using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    [SerializeField] private BossAttackConfig[] attackConfigs;

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private GameObject player;
    private Slider healthBar, staminaBar;
    private BossAttackConfig currentAttack;
    private bool isGrounded = true;
    private bool isBusy = false;
    private bool isAttackInterrupted = false;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float airControlMultiplier = 0.6f;
    [SerializeField] public float transitionSpeed = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -20f;
    private readonly float minVerticalVelocity = -2f;

    [Header("AI Decision Settings")]
    [SerializeField] [Range(0.5f, 1f)] private float damageDuration = 0.5f;
    [SerializeField] [Range(0.5f, 1f)] private float retreatDuration = 1f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        healthBar = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Slider>();
        staminaBar = GameObject.FindGameObjectWithTag("BossStamina").GetComponent<Slider>();
    }

    void Update()
    {
        if (player == null) return;
        ApplyGravity();

        if (isBusy) return;
        if (isAttackInterrupted) return;
        if(currentAttack == null) StartCoroutine(DecideAttackRoutine());

        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool isPlayerInAttackRange = distance <= currentAttack.range;
        Debug.Log($"Attack Range: {currentAttack.range}");
        if (isPlayerInAttackRange) StartCoroutine(AttackRoutine());
        else StartCoroutine(ApproachPlayerRoutine());
    }

    void ApplyGravity()
    {
        if (isGrounded && rigidBody.linearVelocityY < minVerticalVelocity)
            rigidBody.linearVelocityY = minVerticalVelocity;
        else
            rigidBody.linearVelocityY += gravity * Time.deltaTime;
    }

    private IEnumerator DecideAttackRoutine() {
        int attackIndex = Random.Range(0, attackConfigs.Length);
        currentAttack = attackConfigs[attackIndex];
        yield return null;
    }

    private IEnumerator ApproachPlayerRoutine()
    {
        Vector2 direction = LookingDirection();
        LookAtDirection(direction.x);

        UpdateMovementAnimation(currentAttack.isRun ? BossState.Run : BossState.Walk);
        MoveAtDirection(direction.x, currentAttack.isRun ? runSpeed : walkSpeed);
        yield return null;
    }

    private IEnumerator AttackRoutine()
    {
        if (staminaBar.value <= 0f) yield break;
        isBusy = true;
        isAttackInterrupted = false;
    
        UpdateMovementAnimation(BossState.Idle);
        StopMovement();

        animator.SetInteger(StateHash, (int) currentAttack.state);
        animator.SetTrigger(OnAttackHash);

        yield return null;
    }

    public IEnumerator PostAttackRoutine()
    {
        if (isAttackInterrupted) yield break;
        staminaBar.value -= currentAttack.stamina / 100f;

        yield return player.GetComponent<PlayerController>().DamageRoutine(currentAttack);
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

    public IEnumerator DamageRoutine()
    {
        if (healthBar.value <= 0f) yield break;
        healthBar.value -= 0.1f;

        isAttackInterrupted = true;
        isBusy = true;
        
        Vector2 direction = LookingDirection();
        Vector2 knockedDirection = direction * -3f;
        rigidBody.AddForce(knockedDirection, ForceMode2D.Impulse);
        
        animator.SetInteger(StateHash, (int) BossState.Hurt);
        yield return new WaitForSeconds(damageDuration);

        yield return StartCoroutine(RetreatRoutine());
    }
    
    public IEnumerator PostDamageRoutine() {
        if (healthBar.value > 0f) yield break;

        isBusy = true;
        StopMovement();

        healthBar.value = 0f;
        animator.SetInteger(StateHash, (int) BossState.Dead);

        yield return null;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector2 bossPos = transform.position;
        Vector2 playerPos = player.transform.position;

        // Combat distance
        if (currentAttack != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bossPos, currentAttack.range);   
        }

        // Direction to player
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bossPos, playerPos);
    }

    private void UpdateMovementAnimation(BossState state)
    {
        if(state == BossState.Run)
            animator.SetFloat(MoveHash, 1f);
        else if(state == BossState.Walk)
            animator.SetFloat(MoveHash, 0.5f);
        else
            animator.SetFloat(MoveHash, 0f);
        
        animator.SetInteger(StateHash, (int) state);
    }

    private Vector2 LookingDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 bossPosition = transform.position;
        return (playerPosition - bossPosition).normalized;
    }

    private void LookAtDirection(float direction)
    {
        if (direction > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void MoveAtDirection(float direction, float speed)
    {
        rigidBody.linearVelocity = new Vector2(direction * speed, rigidBody.linearVelocity.y);
    }

    private void StopMovement()
    {
        rigidBody.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }

}