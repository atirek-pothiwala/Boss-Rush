using System.Collections;
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
    [SerializeField] private float attackDistance = 0.6f;

    [SerializeField] [Range(0.5f, 1f)] private float idleDuration = 1f;
    [SerializeField] [Range(0.5f, 1f)] private float damageDuration = 0.5f;

    private bool isBusy = false;

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
        if (enemy == null) return;
        if(isBusy) return;
        ApplyGravity();
        HandleJump();
        HandleMovement();
        animator.SetBool(IsGroundedHash, stateController.IsGrounded);
        animator.SetInteger(StateHash, (int) stateController.CurrentState);
    }

    public IEnumerator AttackRoutine()
    {
        if (staminaBar.value < 0.1f) yield break;
        staminaBar.value -= 0.1f;
        
        Vector2 playerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;
        float distance = Vector2.Distance(playerPos, enemyPos);
        
        if (distance > attackDistance) yield break;
        yield return enemy.GetComponent<BossController>().DamageRoutine();   
    }

    public IEnumerator DamageRoutine(BossAttackConfig attackConfig)
    {
        if (healthBar.value <= 0f) yield return null;
        healthBar.value -= attackConfig.damage / 100f;
        
        Vector2 playerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;
        Vector2 knockedDirection = (playerPos - enemyPos).normalized;
        
        isBusy = true;
        
        rigidBody.AddForce(knockedDirection * attackConfig.knockbackForce, ForceMode2D.Impulse);
        animator.SetInteger(StateHash, (int) PlayerState.Hurt);
        yield return new WaitForSeconds(damageDuration);

        isBusy = false;
    }

    void OnDrawGizmosSelected()
    {
        if (enemy == null) return;

        Vector2 playerPos = transform.position;
        Vector2 enemyPos = enemy.transform.position;

        // Combat distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPos, attackDistance);

        // Direction to enemy
        Gizmos.color = Color.green;
        Gizmos.DrawLine(enemyPos, playerPos);
    }
}
