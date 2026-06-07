using UnityEngine;
[RequireComponent(typeof(Animator))]

[RequireComponent(typeof(PlayerStateController))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    [SerializeField] private GameObject character;
    private Animator animator;
    private Rigidbody2D rigidBody;
    private PlayerStateController stateController;

    [Header("Speed Settings")]
    [SerializeField] private float walkSpeed = 1.5f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float airControlMultiplier = 0.6f;
    [SerializeField] public float transitionSpeed = 0.2f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -10f;

    private readonly float minVerticalVelocity = -2f;

    void Awake()
    {
        animator = character.GetComponent<Animator>();
        stateController = GetComponent<PlayerStateController>();
        rigidBody = GetComponent<Rigidbody2D>();
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

}
