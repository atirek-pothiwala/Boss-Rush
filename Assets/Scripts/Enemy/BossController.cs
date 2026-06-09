using UnityEngine;

public class BossController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int MoveHash = Animator.StringToHash("Move");

    [Header("Object References")]
    [SerializeField] private GameObject character;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float airControlMultiplier = 0.6f;
    [SerializeField] public float transitionSpeed = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -20f;
    private readonly float minVerticalVelocity = -2f;

    [Header("AI Decision Settings")]
    [SerializeField] private float combatDistance = 2f;

    [Header("Combat Settings")]
    [SerializeField] private float attackDelayTime = 0.5f;
    [SerializeField] private float postAttackRetreatTime = 0.3f;
    [SerializeField] private int rageModeThresholdPercent = 50;

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private GameObject player;

    // State tracking
    private BossState currentState = BossState.Idle;
    private BossState previousState = BossState.Idle;

    // Movement
    private Vector2 moveDestination = Vector2.zero;
    private bool isGrounded = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;
        ApplyGravity();
        //MoveTowardTarget(player.transform.position, combatDistance);
        //ApplyMovement();
        LookAtPlayer();

        // Update animations
        UpdateAnimations();
    }

    void MoveTowardTarget(Vector2 target, float stopDistance)
    {
        Vector2 currentPos = rigidBody.position;
        Vector2 direction = (target - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, target);

        if (distance > stopDistance)
        {
            moveDestination = new Vector2(direction.x, 0f);
        }
        else
        {
            moveDestination = Vector2.zero;
        }
    }

    void MoveAwayFromTarget(Vector2 target, float minDistance)
    {
        Vector2 currentPos = rigidBody.position;
        Vector2 direction = (currentPos - target).normalized;
        float distance = Vector2.Distance(currentPos, target);

        if (distance < minDistance)
        {
            moveDestination = new Vector2(direction.x, 0f);
        }
        else
        {
            moveDestination = Vector2.zero;
        }
    }

    void ApplyMovement()
    {
        if (!isGrounded)
            return;

        float moveSpeed = currentState == BossState.Run ? runSpeed : walkSpeed;
        Vector2 newVelocity = new(moveDestination.x * moveSpeed, rigidBody.linearVelocityY);
        rigidBody.linearVelocity = newVelocity;
    }

    void ApplyGravity()
    {
        if (isGrounded && rigidBody.linearVelocityY < minVerticalVelocity)
        {
            rigidBody.linearVelocityY = minVerticalVelocity;
        }
        else
        {
            rigidBody.linearVelocityY += gravity * Time.deltaTime;
        }
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector2(-1f, 1f);
        }
        else
        {
            transform.localScale = new Vector2(1f, 1f);
        }
    }

    void UpdateAnimations()
    {
        // Update state
        if (currentState != previousState)
        {
            animator.SetInteger(StateHash, (int)currentState);
            previousState = currentState;
        }

        // Update movement blend
        animator.SetFloat(MoveHash, Mathf.Abs(moveDestination.x), transitionSpeed, Time.deltaTime);
    }

    public void OnAttack()
    {
        if (player == null) return;
        Vector2 playerPos = player.transform.position;
        Vector2 enemyPos = rigidBody.position;;
        if (Vector2.Distance(enemyPos, playerPos) > combatDistance) return;
        player.GetComponent<PlayerController>().OnHurt();
    }

    public void RunAway()
    {
        if (player == null) return;
        currentState = BossState.Run;
        //MoveAwayFromTarget(player.transform.position, retreatDistance);
    }

    public void OnHurt()
    {
        if (player == null) return;
        Vector2 playerPos = player.transform.position;
        Vector2 bossPos = rigidBody.position;

        // Deal damage
        Vector2 knocked = (bossPos - playerPos).normalized * 3f;
        rigidBody.AddForce(knocked, ForceMode2D.Impulse);
        animator.SetInteger(StateHash, (int) BossState.Hurt);
    }
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector2 bossPos = transform.position;
        Vector2 playerPos = player.transform.position;

        // Combat distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(bossPos, combatDistance);

        // Direction to player
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bossPos, playerPos);
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