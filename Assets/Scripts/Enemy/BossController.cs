using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int MoveHash = Animator.StringToHash("Move");

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private GameObject player;
    private Slider healthBar, staminaBar;

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
        healthBar = GameObject.FindGameObjectWithTag("BossHealth").GetComponent<Slider>();
        staminaBar = GameObject.FindGameObjectWithTag("BossStamina").GetComponent<Slider>();
    }

    void Update()
    {
        if (player == null) return;
        ApplyGravity();
        MoveTowardTarget(player.transform.position, combatDistance);
        ApplyMovement();
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
        if (staminaBar.value < 0.1f) { return; }
        staminaBar.value -= 0.1f;

        if (player == null) return;
        Vector2 playerPos = player.transform.position;
        Vector2 enemyPos = transform.position;
        float distance = Vector2.Distance(playerPos, enemyPos);
        if (distance > combatDistance) return;
        player.GetComponent<PlayerController>().OnDamage();
    }

    public void OnDamage()
    {
        if (healthBar.value <= 0f) { return; }
        healthBar.value -= 0.1f;
        
        if (player == null) return;
        Vector2 playerPos = player.transform.position;
        Vector2 bossPos = transform.position;

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

    public void RunAway()
    {
        if (player == null) return;
        currentState = BossState.Run;
        //MoveAwayFromTarget(player.transform.position, retreatDistance);
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