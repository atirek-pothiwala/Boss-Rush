using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    public static event System.Action<Vector2> OnMovementEvent;
    public static event System.Action<bool> OnRunEvent;
    public static event System.Action<bool> OnShieldEvent;
    public static event System.Action<bool> OnJumpEvent;
    public static event System.Action<bool> OnQuickAttackEvent;
    public static event System.Action<bool> OnPowerAttackEvent;
    public static event System.Action<bool> OnSpecialAttackEvent;
    public static event System.Action<bool> OnPauseEvent;

    public static void SetTouchMovement(Vector2 input) => OnMovementEvent?.Invoke(input);
    public static void SetTouchRun(bool isRunning) => OnRunEvent?.Invoke(isRunning);
    public static void SetTouchShield(bool isShielding) => OnShieldEvent?.Invoke(isShielding);
    public static void SetTouchJump(bool isJumping) => OnJumpEvent?.Invoke(isJumping);
    public static void SetTouchQuickAttack(bool isAttacking) => OnQuickAttackEvent?.Invoke(isAttacking);
    public static void SetTouchHeavyAttack(bool isAttacking) => OnPowerAttackEvent?.Invoke(isAttacking);
    public static void SetTouchSpecialAttack(bool isAttacking) => OnSpecialAttackEvent?.Invoke(isAttacking);

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += ctx => OnMovementEvent?.Invoke(ctx.ReadValue<Vector2>());
            playerControls.PlayerMovement.Movement.canceled += _ => OnMovementEvent?.Invoke(Vector2.zero);

            playerControls.PlayerActions.Run.performed += ctx => OnRunEvent?.Invoke(true);
            playerControls.PlayerActions.Run.canceled += ctx => OnRunEvent?.Invoke(false);

            playerControls.PlayerActions.Jump.performed += ctx => OnJumpEvent?.Invoke(true);
            playerControls.PlayerActions.Jump.canceled += ctx => OnJumpEvent?.Invoke(false);

            playerControls.PlayerActions.QuickAttack.performed += ctx => OnQuickAttackEvent?.Invoke(true);
            playerControls.PlayerActions.QuickAttack.canceled += ctx => OnQuickAttackEvent?.Invoke(false);

            playerControls.PlayerActions.PowerAttack.performed += ctx => OnPowerAttackEvent?.Invoke(true);
            playerControls.PlayerActions.PowerAttack.canceled += ctx => OnPowerAttackEvent?.Invoke(false);

            playerControls.PlayerActions.PowerUp.performed += ctx => OnSpecialAttackEvent?.Invoke(true);
            playerControls.PlayerActions.PowerUp.canceled += ctx => OnSpecialAttackEvent?.Invoke(false);

            playerControls.PlayerActions.Shield.performed += ctx => OnShieldEvent?.Invoke(true);
            playerControls.PlayerActions.Shield.canceled += ctx => OnShieldEvent?.Invoke(false);

            playerControls.OtherActions.Pause.performed += ctx => OnPauseEvent?.Invoke(true);
        }
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}
