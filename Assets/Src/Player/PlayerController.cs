using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int OnActionHash = Animator.StringToHash("OnAction");

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody;
    private GameObject boss;
    private SoundManager SoundManager => SoundManager.Instance;
    private PauseManager PauseManager => PauseManager.Instance;
    private HealthManager HealthManager => HealthManager.Instance;
    public bool IsPreventActions => PauseManager.IsGamePaused || HealthManager.IsGameOver;

    private bool isBusy = false;
    private bool isSprint = false;
    private bool isGrounded = false;
    private Vector2 movementInput;

    [Header("Speed Settings")]
    [SerializeField] [Range(1f, 2f)] private float walkSpeed = 1.5f;
    [SerializeField] [Range(2f, 4f)] private float runSpeed = 3f;
    [SerializeField] private float airControlMultiplier = 0.6f;
    [SerializeField] public float transitionSpeed = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -10f;
    private readonly float minVerticalVelocity = -2f;

    [Header("Attack Settings")]
    [SerializeField] private PlayerAttackConfig[] attacks;
    private PlayerAttackConfig currentAttack;
    private float lastShieldTime = 0f;
    [SerializeField] [Range(0.1f, 0.5f)] float shieldCooldown = 0.25f;
    private bool IsShieldEngaged => lastShieldTime + shieldCooldown >= Time.time;
    [SerializeField] [Range(0.5f, 2f)] float damageCooldown = 1f;
    private float RangeScale => boss.transform.localScale.z;

    void OnEnable()
    {
        PlayerInputManager.OnMovementEvent += OnMovementEvent;
        PlayerInputManager.OnRunEvent += OnRunEvent;
        PlayerInputManager.OnQuickAttackEvent += OnQuickAttackEvent;
        PlayerInputManager.OnPowerAttackEvent += OnHeavyAttackEvent;
        PlayerInputManager.OnJumpEvent += OnJumpEvent;
        PlayerInputManager.OnShieldEvent += OnShieldEvent;
        PlayerInputManager.OnPowerUpEvent += OnPowerUpEvent;
    }

    void OnDisable()
    {
        PlayerInputManager.OnMovementEvent -= OnMovementEvent;
        PlayerInputManager.OnRunEvent -= OnRunEvent;
        PlayerInputManager.OnQuickAttackEvent -= OnQuickAttackEvent;
        PlayerInputManager.OnPowerAttackEvent -= OnHeavyAttackEvent;
        PlayerInputManager.OnJumpEvent -= OnJumpEvent;
        PlayerInputManager.OnShieldEvent -= OnShieldEvent;
        PlayerInputManager.OnPowerUpEvent -= OnPowerUpEvent;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        lastShieldTime = Time.time;
    }

    void Update()
    {
        if (PauseManager.IsGamePaused) return;
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("Boss");
            return;
        }
        ApplyGravity();
        
        if (HealthManager.IsGameOver)
        {
            if(HealthManager.IsBossDead) StopMovement();
            return;
        }
        if(isBusy) return;
        ApplyMovement();
    }

    public void ApplyGravity()
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

    void ApplyMovement()
    {
        Vector2 moveInput = movementInput.normalized;
        float moveSpeed = isSprint ? runSpeed : walkSpeed;
        float moveAmount = isSprint ? 2f : moveInput.magnitude;

        MoveAtDirection(moveInput.x, moveSpeed);
        LookAtDirection(moveInput.x);
        animator.SetFloat(MoveHash, moveAmount, transitionSpeed, Time.deltaTime);
    }

    void StopMovement()
    {
        animator.SetFloat(MoveHash, 0, transitionSpeed, Time.deltaTime);
    }


    // Input Event Handlers
    private void OnMovementEvent(Vector2 input)
    {
        if (IsPreventActions) return;
        input.y = 0f; // Ensure vertical input doesn't affect movement state
        movementInput = input;
    }

    void OnRunEvent(bool isRunning)
    {
        if (IsPreventActions) return;
        isSprint = isRunning && movementInput.magnitude > 0f;
    }
    
    void OnQuickAttackEvent(bool isAttacking)
    {
        if (IsPreventActions) return;
        if (!isAttacking || !isGrounded) return;

        currentAttack = attacks.First(attack => attack.state == PlayerState.QuickAttack);
        StartCoroutine(AttackRoutine());
    }

    void OnHeavyAttackEvent(bool isAttacking)
    {
        if (IsPreventActions) return;
        if (!isAttacking || !isGrounded) return;

        currentAttack = attacks.First(attack => attack.state == PlayerState.HeavyAttack);
        StartCoroutine(AttackRoutine());
    }
    
    void OnSpecialAttackEvent(bool isAttacking)
    {
        if (IsPreventActions) return;
        if (!isAttacking || !isGrounded) return;

        currentAttack = attacks.First(attack => attack.state == PlayerState.SpecialAttack);
        StartCoroutine(AttackRoutine());
    }

    void OnPowerUpEvent(bool isPoweringUp)
    {
        if (IsPreventActions) return;
    }

    void OnJumpEvent(bool isJumping)
    {
        if (IsPreventActions) return;
        if (!isJumping || !isGrounded) return;

        isBusy = true;
        animator.SetInteger(StateHash, (int) PlayerState.Jump);
        animator.SetTrigger(OnActionHash);
    }

    void OnShieldEvent(bool isShielding)
    {
        if (IsPreventActions) return;
        if (!isShielding || IsShieldEngaged) return;
        lastShieldTime = Time.time;

        animator.SetInteger(StateHash, (int) PlayerState.Shield);
        animator.SetTrigger(OnActionHash);
    }


    // Routines

    public IEnumerator JumpRoutine()
    {
        isBusy = false;
        rigidBody.linearVelocityY = Mathf.Sqrt(jumpHeight * minVerticalVelocity * gravity);
        yield return null;
    }


    public IEnumerator AttackRoutine()
    {
        if (currentAttack == null) yield break;
        if (HealthManager.IsHeroStaminaDepleted) yield break;
        if (HealthManager.HeroStamina < currentAttack.stamina) yield break;
        
        animator.SetInteger(StateHash, (int) currentAttack.state);
        animator.SetTrigger(OnActionHash);
    }

    public IEnumerator PostAttackRoutine()
    {
        HealthManager.UpdateHeroStamina(-currentAttack.stamina);

        float distance = DistanceToBoss();
        if (distance <= currentAttack.range * RangeScale)
        {
            StartCoroutine(boss.GetComponent<BossController>().DamageRoutine(currentAttack));
        } 
        else
        {
            SoundManager.PlayBlankAttack();
        }
        yield return new WaitForSeconds(currentAttack.cooldown);
    }

    public IEnumerator DamageRoutine(BossAttackConfig attack)
    {
        if (HealthManager.IsHeroDead) yield return null;
        SoundManager.PlayOneShot(attack.hitSound);

        isBusy = true;
        Vector2 direction = LookingDirection();

        if(IsShieldEngaged)
        {
            rigidBody.AddForceX(-direction.x * attack.knockbackForce / 2, ForceMode2D.Impulse);
            HealthManager.UpdateHeroStamina(attack.stamina / 2);
        } 
        else
        {
            HealthManager.UpdateHeroHealth(-attack.damage);
            rigidBody.AddForceX(-direction.x * attack.knockbackForce, ForceMode2D.Impulse);
            animator.SetInteger(StateHash, (int) PlayerState.Hurt);
            animator.SetTrigger(OnActionHash);
        }

        if (HealthManager.IsHeroDead)
        {
            animator.SetInteger(StateHash, (int) PlayerState.Death);
            SoundManager.PlayGameOver();
            yield break;
        }
        
        yield return new WaitForSeconds(damageCooldown);

        isBusy = false;
    }


    // Helper Methods

    private void MoveAtDirection(float direction, float speed)
    {
        float airMultiplier = isGrounded ? 1f : airControlMultiplier;
        Vector2 moveVelocity = new(direction * speed * airMultiplier, rigidBody.linearVelocityY);
        rigidBody.linearVelocity = moveVelocity;
    }

    private void LookAtDirection(float direction)
    {
        if (direction > 0)
            transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        else if (direction < 0)
            transform.localScale = new Vector3(-1.4f, 1.4f, 1.4f);
    }

    private Vector2 LookingDirection()
    {
        Vector2 bossPosition = boss.transform.position;
        Vector2 heroPosition = transform.position;
        return (bossPosition - heroPosition).normalized;
    }

    private float DistanceToBoss()
    {
        Vector2 playerPos = transform.position;
        Vector2 bossPos = boss.transform.position;
        return Vector2.Distance(playerPos, bossPos);
    }

    void OnDrawGizmosSelected()
    {
        if (boss == null) return;

        Vector2 playerPos = transform.position;
        Vector2 enemyPos = boss.transform.position;

        if (currentAttack != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerPos, currentAttack.range * RangeScale);   
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(enemyPos, playerPos);
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
