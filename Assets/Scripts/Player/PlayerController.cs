using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private PlayerStateController stateController;
    private GameObject enemy;
    private Slider healthBar, staminaBar;


    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float airControlMultiplier = 0.6f;
    [SerializeField] public float transitionSpeed = 0.2f;


    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -10f;
    private readonly float minVerticalVelocity = -2f;


    [Header("Attack Settings")]
    [SerializeField] private float combatDistance = 0.6f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        stateController = GetComponent<PlayerStateController>();
        rigidBody = GetComponent<Rigidbody2D>();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        healthBar = GameObject.FindGameObjectWithTag("HeroHealth").GetComponent<Slider>();
        staminaBar = GameObject.FindGameObjectWithTag("HeroStamina").GetComponent<Slider>();
    }

    public void ApplyGravity()
    {
        if (stateController.IsGrounded && rigidBody.linearVelocityY < minVerticalVelocity)
        {
            rigidBody.linearVelocityY = minVerticalVelocity;
        }
        else
        {
            rigidBody.linearVelocityY += gravity * Time.deltaTime;
        }
        stateController.RefreshState();
    }

    void HandleMovement()
    {
        Vector2 moveInput = stateController.MovementInput.normalized;
        float moveSpeed = stateController.OnRun ? runSpeed : walkSpeed;
        float moveAmount = stateController.OnRun ? 2f : moveInput.magnitude;

        // Apply horizontal movement with air control
        float airMultiplier = stateController.IsGrounded ? 1f : airControlMultiplier;
        Vector2 moveVelocity = new(moveInput.x * moveSpeed * airMultiplier, rigidBody.linearVelocityY);
        rigidBody.linearVelocity = moveVelocity;

        // Flip character sprite based on input direction
        if (moveInput.x != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1f, 1f);
        }

        // Update animation
        animator.SetFloat(MoveHash, moveAmount, transitionSpeed, Time.deltaTime);
    }

    void HandleJump()
    {
        if (!stateController.OnJump || !stateController.IsGrounded) return;
        
        // Calculate jump velocity from jump height
        float jumpVelocity = Mathf.Sqrt(jumpHeight * minVerticalVelocity * gravity);

        // Set vertical velocity for jump
        rigidBody.linearVelocityY = jumpVelocity;
    }

    void Update()
    {
        HandleJump();
        ApplyGravity();
        HandleMovement();

        animator.SetBool(IsGroundedHash, stateController.IsGrounded);
        animator.SetInteger(StateHash, (int) stateController.CurrentState);
    }

    public void OnAttack()
    {
        if (staminaBar.value < 0.1f) { return; }
        staminaBar.value -= 0.1f;

        if (enemy == null) return;
        Vector2 playerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;
        float distance = Vector2.Distance(playerPos, enemyPos);
        if (distance > combatDistance) return;
        enemy.GetComponent<BossController>().OnDamage();
    }

    public void OnDamage()
    {
        if (healthBar.value <= 0f) { return; }
        healthBar.value -= 0.1f;
        
        if (enemy == null) return;
        Vector2 playerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;

        // Deal damage
        Vector2 knocked = (enemyPos - playerPos).normalized * 3f;
        rigidBody.AddForce(knocked, ForceMode2D.Impulse);
        animator.SetInteger(StateHash, (int) PlayerState.Hurt);
    }

    void OnDrawGizmosSelected()
    {
        if (enemy == null) return;

        Vector2 playerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;

        // Combat distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPos, combatDistance);

        // Direction to enemy
        Gizmos.color = Color.green;
        Gizmos.DrawLine(enemyPos, playerPos);
    }
}
